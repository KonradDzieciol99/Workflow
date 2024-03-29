﻿using Microsoft.AspNetCore.Mvc.Testing;
using Respawn;
using Testcontainers.MsSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Bogus;
using Chat.Domain.Entity;
using MessageBus;
using Moq;
using Microsoft.Extensions.Configuration;
using TestsHelpers.Extensions;
using MessageBus.Models;

namespace Chat.IntegrationTests;

[CollectionDefinition("Base")]
public class WebApplicationFactoryCollection : ICollectionFixture<Base> { }

public class Base : IAsyncLifetime
{
    public readonly WebApplicationFactory<Program> _factory;
    public HttpClient _client = null!;
    public Respawner _checkpoint = null!;
    public readonly MsSqlContainer _msSqlContainer;

    public Base()
    {
        _msSqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .Build();

        this._factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration(
                (context, configBuilder) =>
                {
                    configBuilder.AddInMemoryCollection(
                        new Dictionary<string, string?>
                        {
                            ["isTest"] = "true",
                            ["RabbitMQOptions:RabbitMQConnectionString"] = "test",
                            ["RabbitMQOptions:Exchange"] = "test",
                            ["RabbitMQOptions:Queue"] = "test",
                        }
                    );
                }
            );

            builder.ConfigureServices(
                (context, services) =>
                {
                    var mockSender = new Mock<IEventBusSender>();
                    var mockConsumer = new Mock<IEventBusConsumer>();

                    mockSender
                        .Setup(sender => sender.PublishMessage(It.IsAny<IntegrationEvent>()))
                        .Returns(Task.CompletedTask);
                    mockConsumer
                        .Setup(consumer => consumer.Subscribe<IntegrationEvent>())
                        .Returns(Task.CompletedTask);

                    services.AddSingleton<IEventBusSender>(mockSender.Object);
                    services.AddSingleton<IEventBusConsumer>(mockConsumer.Object);

                    services.Remove<DbContextOptions<ApplicationDbContext>>();

                    var dbConnString = _msSqlContainer.GetConnectionString();
                    services.AddDbContext<ApplicationDbContext>(
                        options =>
                            options.UseSqlServer(
                                dbConnString,
                                builder =>
                                    builder.MigrationsAssembly(
                                        typeof(ApplicationDbContext).Assembly.FullName
                                    )
                            )
                    );

                    services
                        .AddAuthentication(opt =>
                        {
                            opt.DefaultAuthenticateScheme = TestAuthHandler.AuthenticationScheme;
                            opt.DefaultChallengeScheme = TestAuthHandler.AuthenticationScheme;
                        })
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                            TestAuthHandler.AuthenticationScheme,
                            options => { }
                        );
                    services.AddAuthorization(options =>
                    {
                        options.AddPolicy(
                            "ApiScope",
                            policy =>
                            {
                                policy.RequireAssertion(context => true);
                            }
                        );
                    });
                }
            );
        });
    }

    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();

        _client = _factory.CreateClient();

        _checkpoint = await Respawner.CreateAsync(
            _msSqlContainer.GetConnectionString(),
            new RespawnerOptions
            {
                TablesToIgnore = new Respawn.Graph.Table[] { "__EFMigrationsHistory" }
            }
        );
    }

    public async Task DisposeAsync()
    {
        await _msSqlContainer.DisposeAsync().AsTask();
    }

    public static List<FriendRequest> GetFakeFriendRequests(
        int amount = 1,
        bool confirmed = false,
        string? staticInviterUserId = null,
        string? staticInviterUserEmail = null,
        string? staticInvitedUserId = null,
        string? staticInvitedUserEmail = null
    )
    {
        return new Faker<FriendRequest>()
            .StrictMode(false)
            .CustomInstantiator(
                f =>
                    new FriendRequest(
                        staticInviterUserId is null
                            ? Guid.NewGuid().ToString()
                            : staticInviterUserId,
                        staticInviterUserEmail is null
                            ? f.Internet.Email()
                            : staticInviterUserEmail,
                        null,
                        staticInvitedUserId is null
                            ? Guid.NewGuid().ToString()
                            : staticInvitedUserId,
                        staticInvitedUserEmail is null
                            ? f.Internet.Email()
                            : staticInvitedUserEmail,
                        null
                    )
            )
            .FinishWith(
                (f, friendRequest) =>
                {
                    if (confirmed)
                        friendRequest.AcceptRequest(friendRequest.InvitedUserId);
                }
            )
            .Generate(amount);
    }
}
