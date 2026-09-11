# PersonalAIAgent

PersonalAIAgent is a Windows-based AI assistant built with .NET 10 MAUI Blazor Hybrid. It acts as a bridge between natural language and your local machine by translating your requests into Windows Command Prompt (CMD) commands using Google's Gemini Flash model, and then executing them directly on your system.

## ✨ Features

* **Natural Language to CMD**: Type what you want to do in plain English, and the agent will generate and run the appropriate CMD command.
* **Bring Your Own Key (BYOK)**: Connects to the free Google AI Studio API. Simply generate an API key and insert it into the application.
* **Smart Auto-Correction**: The underlying prompt is designed to automatically correct misspelled application names before generating the command.
* **Local Storage**: Securely stores your API key using SQLite and Entity Framework Core.
* **Minimalist UI**: Features a clean, two-page interface designed entirely in Blazor for a seamless Windows desktop experience.

## 🛠️ Tech Stack

* **Framework**: .NET 10, .NET MAUI (Windows target only)
* **UI**: Blazor Hybrid
* **Database**: SQLite
* **ORM**: Entity Framework Core
* **AI Provider**: Google Gemini Flash (via Google AI Studio)

## 🏗️ Project Structure

The solution is divided into two main projects to keep the logic separated from the presentation layer:

### 1. `PersonalAIAgent.Core`
Handles all the business logic, database interactions, and API communication.
* `/Data`: Contains the Entity Framework Core Context and database entities.
* `/Interfaces`: Contains a single, unified interface that maps out all core logic, including calling the Gemini agent, and CRUD operations for the API key (Add, Edit, Delete, and Get).
* `/Models`: Serves as the Data Transfer Objects (DTOs) for passing data cleanly between layers.
* `/Services`: Contains `AgentService`, the workhorse of the application where the core logic and AI communication are implemented.

### 2. `PersonalAIAgent.App`
The .NET MAUI solution that serves as the presentation layer.
* `/Components/Pages`: Contains the Blazor UI/UX implementation. The application is kept lightweight with only two primary pages for managing the API key and interacting with the agent.

## ⚙️ How It Works

When you submit a request, `PersonalAIAgent` packages your input and sends it to Gemini Flash with a strict system prompt to ensure safe, executable outputs:

> *"This request is sent from a custom C# agent running on Windows. The result must be a command that will execute the task in CMD. Since your output will be run directly in the Windows Command Prompt, please provide only the raw command without any additional words or explanations. If the application name is misspelled, please correct it automatically."*

The application takes the raw string returned by Gemini and passes it directly to the local Windows Command Prompt for execution.

## 🚀 Getting Started

### Prerequisites
* Visual Studio 2022 (with .NET MAUI workload installed)
* .NET 10 SDK
* A free API key from [Google AI Studio](https://aistudio.google.com/)

### Installation
1. Clone the repository:
   ```bash
   git clone [https://github.com/ALB1381/PersonalAIAgent.git](https://github.com/ALB1381/PersonalAIAgent.git)
   If you want to publish the project as release you can use this command:
2. Add migration adn update data base: If you wanted to add a migration or update DB please change <TargetFrameworks>net10.0-windows10.0.19041.0</TargetFrameworks> to <TargetFramework>net10.0-windows10.0.19041.0</TargetFramework>
     ```bash
     dotnet ef migrations add InitialSqliteCreate --project PersonalAIAgent.Core
     dotnet ef database update --project PersonalAIAgent.Core --startup-project PersonalAIAgent.App
3. Publish project Or run it in visual studio:
   3.1: Run it in visual studio by running the App project
   3.2: Create the exe file by this command 
   ```bash
   dotnet publish PersonalAIAgent.App\PersonalAIAgent.App.csproj -f net10.0-windows10.0.19041.0 -c Release -r win-x64 -p:WindowsPackageType=None -p:WindowsAppSDKSelfContained=true --self-contained true
4. You can run the exe file by open \PersonalAIAgent.App\bin\Release\net10.0-windows10.0.19041.0\win-x64\PersonalAIAgent.App.exe
and here is the windows app to run the application.
