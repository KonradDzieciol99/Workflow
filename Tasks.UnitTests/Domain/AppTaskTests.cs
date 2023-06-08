using Microsoft.Azure.Amqp.Framing;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tasks.Domain.Common.Models;
using Tasks.Domain.Entity;
using Tasks.Domain.Exceptions;

namespace Tasks.UnitTests.Domain;

public class AppTaskTests
{

    [Fact]
    public void UpdateTask_NoChanges_ThrowsTaskDomainException()
    {
        // Arrange
        var task = GetFakeAppTask();
        // Act  
        var action = () => task.UpdateTask("test", null, null, null, null, Priority.Low, State.ToDo, new DateTime(1900, 12, 1), new DateTime(1899, 12, 1), null);
        // Assert
        var ex = Assert.Throws<TaskDomainException>(action);
    }

    [Fact]
    public void UpdateTask_ChangesValues_SuccessfullyUpdatesValues()
    {
        // Arrange
        var task = GetFakeAppTask();
        var newName = "New name";
        var newDescription = "New description";
        var newTaskAssigneeMemberId = "New assignee member id";
        var newTaskAssigneeMemberEmail = "New assignee email";
        var newTaskAssigneeMemberPhotoUrl = "New assignee photo URL";
        var newPriority = Priority.High;
        var newState = State.Done; 
        var newDueDate = new DateTime(1900, 12, 1);
        var newStartDate = new DateTime(1900, 12, 1).AddDays(1);
        var newTaskLeaderId = "New task leader id";
        // Act  
        task.UpdateTask(newName, newDescription, newTaskAssigneeMemberId, newTaskAssigneeMemberEmail, newTaskAssigneeMemberPhotoUrl, newPriority, newState, newDueDate, newStartDate, newTaskLeaderId);
        // Assert
        Assert.Equal(newName, task.Name);
        Assert.Equal(newDescription, task.Description);
        Assert.Equal(newTaskAssigneeMemberId, task.TaskAssigneeMemberId);
        Assert.Equal(newTaskAssigneeMemberEmail, task.TaskAssigneeMemberEmail);
        Assert.Equal(newTaskAssigneeMemberPhotoUrl, task.TaskAssigneeMemberPhotoUrl);
        Assert.Equal(newPriority, task.Priority);
        Assert.Equal(newState, task.State);
        Assert.Equal(newDueDate, task.DueDate);
        Assert.Equal(newStartDate, task.StartDate);
        Assert.Equal(newTaskLeaderId, task.TaskLeaderId);
    }

    private AppTask GetFakeAppTask()
        => new AppTask("test", null, "A", null, null, null, Priority.Low, State.ToDo, new DateTime(1900, 12, 1), new DateTime(1899, 12, 1), null);
    
}
