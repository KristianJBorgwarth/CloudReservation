# CloudReservation

CloudReservation is a Proof-of-Concept (POC) project that integrates Outlook room booking with a meeting room reservation system using the Microsoft Graph API. This project was developed in collaboration with an IT company as part of the final exam at the AP Graduate course in Computer Science at Dania Erhvervsakademi.

## Table of Contents

- [Description](#description)
- [Features](#features)
- [Technologies](#technologies)
- [Installation](#installation)
  - [Prerequisites](#prerequisites)
  - [Steps](#steps)
  - [Using Docker](#using-docker)
- [Usage](#usage)
  - [API Endpoints](#api-endpoints)
    - [Authentication](#authentication)
    - [Users](#users)
    - [Rooms and Events (Microsoft Graph)](#rooms-and-events-microsoft-graph)

## Description

The CloudReservation API facilitates the integration of Outlook in a meeting room booking application. It offers real-time updates through SignalR and Microsoft Graph Webhooks (Change notifications).

## Features

- Integration with Microsoft Outlook for room booking.
- Real-time updates using SignalR and Microsoft Graph Webhooks.
- JWT-based authentication.
- CRUD operations for users and claims.
- Event management in Outlook calendars.

## Technologies

- ASP.NET Core
- Entity Framework Core
- Microsoft Graph API
- SignalR
- JWT Authentication
- Docker

## Installation

### Prerequisites

- .NET 6 SDK
- Docker
- MySQL or MariaDB
- Access to Microsoft Graph API through relevant Microsoft account

### Steps

1. **Clone the repository:**
   ```sh
   git clone https://github.com/KristianJBorgwarth/CloudReservation.git
   cd CloudReservation
   ```
   
2. **Set up the database:**
   ```sh
   docker-compose up -d mariadb
   ```

3. **Set environment variables for MS Graph connection:**
   Alter them in appsettings.json or through github secrets
   
4. **Run the application:**
   ```sh
   dotnet run --project CloudReservation.Service
   ```
   
## Usage

### API Endpoints

#### Authentication

- **POST** `/api/auth/login`: Logs in a user and returns a JWT token.

#### Users

- **POST** `/api/user/create`: Creates a new user.
- **GET** `/api/user/users`: Gets all users.
- **GET** `/api/user/user`: Gets a user by username.
- **DELETE** `/api/user/delete`: Deletes a user by username.
- **PUT** `/api/user/update`: Updates a user's claims.

#### Rooms and Events (Microsoft Graph)

- **GET** `/api/msgraph/rooms`: Retrieves a list of bookable rooms in Outlook.
- **GET** `/api/msgraph/events`: Retrieves a list of events for a specified room and date.
- **POST** `/api/msgraph/create-event`: Creates a new event in the specified room calendar.
- **POST** `/api/msgraph/delete-event`: Deletes an event in the specified room calendar.
