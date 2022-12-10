# Fortnite Notifier

# Description

The Fortnite Notifier is a collection of projects to help inform individuals about updates to Fortnite.

# Projects

The following projects are currently present:

## Fortnite Notifier

The Fortnite Notifier is a worker service that will poll the fortnite status page to check for updates. Once an update is complete it will send a notification to all subscribed users.

## Fortnite Notifier Admin

The admin site is a web interface to adjust settings for the notifier. It will allow users to unsubscribe from notifications for admin users to add new users to be notified.

# Developer Setup

## Prerequisites

The following prerequisites are required to run the projects:

  * Visual Studio 2022
  * .NET 7 SDK
  * PostgreSQL 14

The Fortnite Notifier projects will be developed locally using native .NET Core tooling. Configuration for the projects will be stored in a combination of the `appsettings.json` and a `secrets.json` file per project. Please see the 'templates' folder for examples of the secrets.json file.

# Production Setup

The Fortnite Notifier projects will be run in a dockerised production environment. The docker-compose file included in the repository will run the projects locally. For production configuration a combination of `.env` files will be used.

Shared settings will exist in a `shared.env` file, while project specific settings will exist in a `.env` file located in a folder with the same name as the project. Please see the 'templates' folder for examples of the .env files.