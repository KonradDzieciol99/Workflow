# Workflow
#### Project Management and Real-time Communication Application

![](README-img/workflowDiag.drawio.svg)

The application was designed as a system for project management and real-time communication. It consists of several microservices, ensuring scalability and flexibility. Currently, it is in active development and is set to undergo further enhancements.

## Features
- Live Chat: Enables users to engage in real-time conversations using SignalR, facilitating instant message exchange and information sharing.
- Simple Project Management System: Users can create projects, invite participants, and assign roles within them. Within each project, there is an option to define and allocate tasks to individual users, streamlining team coordination.
- Friend Addition: A social feature allowing users to add friends and build a contact network within the platform.
  
#### Live Chat 

![](README-img/workflow-messages.png)

#### Projects Page

![](README-img/workflow-projects-page.png)

#### Tasks Page 

![](README-img/worflow-tasks-page-create-task.png)

## Architecture overview

![](README-img/workflowDiag.drawio.17.09.2023.svg)

- Microservices: The microservice-based architecture allows for independent expansion and updates of individual application components.
- Inter-service communication: Use of RabbitMQ and Azure Service Bus for reliable and scalable inter-service communication.
- Security and authentication: Integrated protection and identity management thanks to Duende Identity Server.
- Frontend: User interface based on Angular, served by the nginx server and styled with Bootstrap.

## Getting Started

- Make sure you have docker [installed](https://docs.docker.com/docker-for-windows/install/)
- Download the [Workflow-UI](https://github.com/KonradDzieciol99/WorkflowUI/) frontend
- Use the following commands in the root folder of both projects

```powershell
docker-compose build
docker-compose up
```

You should be able to browse different components of the application by using the below URLs :

```
Health Check : http://localhost:5018/
Seq :  http://localhost:5200/
Workflow :  http://localhost:1000/
```


