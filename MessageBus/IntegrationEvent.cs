﻿using MediatR;
using System;

namespace MessageBus;

public class IntegrationEvent : IRequest
{
    public IntegrationEvent()
    {

    }

    public DateTime MessageCreated { get; set; }
    //public SimpleUser? EventRecipient { get; set; }
    //public string? NotificationPartnerUserId { get; set; }
    //public string? NotificationPartnerUserEmail { get; set; }
    //public string? NotificationPartnerUserPhotoUrl { get; set; }

    //public SimpleUser? EventSender { get; set; } = new SimpleUser() { UserId = "System", UserEmail = "System" };
    public string? EventSenderUserId { get; set; }
    public string? EventSenderUserEmail { get; set; }
    public string? EventSenderUserPhotoUrl { get; set; }

    public string EventType { get; set; }
    public object? ObjectId { get; set; }//streamId

    //public SimpleUser? EventSender { get; set; }
    //w zależności od tego czy zakceptowane czy nie zakceptowane czy oczekujące
    //będzie sie zmieniał po prostu rozaj eventu 
}
