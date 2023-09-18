# Workflow
#### Project Management and Real-time Communication Application

<img src="https://github.com/KonradDzieciol99/Workflow/blob/master/README-img/workflowDiag.drawio.17.09.2023.svg" alt="eShop logo" title="eShopOnContainers" align="right" width="100%" height="auto" />

The application was designed as a system for project management and real-time communication. It consists of several microservices, ensuring scalability and flexibility. Currently, it is in active development and is set to undergo further enhancements.

## Features
- Live Chat: Enables users to engage in real-time conversations using SignalR, facilitating instant message exchange and information sharing.
- Simple Project Management System: Users can create projects, invite participants, and assign roles within them. Within each project, there is an option to define and allocate tasks to individual users, streamlining team coordination.
- Friend Addition: A social feature allowing users to add friends and build a contact network within the platform.

### Live Chat 

<img src="https://github.com/KonradDzieciol99/Workflow/blob/master/README-img/workflow-messages.png" align="right" width="100%" height="auto" />

### Projects Page

<img src="https://github.com/KonradDzieciol99/Workflow/blob/master/README-img/workflow-projects-page.png" align="right" width="100%" height="auto" />

### Tasks Page 

<img src="https://github.com/KonradDzieciol99/Workflow/blob/master/README-img/worflow-tasks-page-create-task.png" align="right" width="100%" height="auto" />

### Architecture overview

<img src="https://github.com/KonradDzieciol99/Workflow/blob/master/README-img/workflowDiag.drawio.17.09.2023.svg" alt="eShop logo" title="eShopOnContainers" align="right" width="100%" height="auto" />

- Microservices: The microservice-based architecture allows for independent expansion and updates of individual application components.
- Inter-service communication: Use of RabbitMQ and Azure Service Bus for reliable and scalable inter-service communication.
- Security and authentication: Integrated protection and identity management thanks to Duende Identity Server.
- Frontend: User interface based on Angular, served by the nginx server and styled with Bootstrap.
