# Dating App

Dating app created using Angular6 and a .NET Core 2.1 API. <br>
This app was created following the tutorial at: https://www.udemy.com/build-an-app-with-aspnet-core-and-angular-from-scratch/learn/v4/overview.

This project consists of an online dating application which allows users to create a profile, edit their profile and message other users. <br>
The front-end Angular application communicates with the back-end C# .NET Core API to authenticate users and store their information in the database. <br>
The database is created using Entity Framework Core. <br>
This application also utilises Bootstrap 4 and various 3rd party libraries, including NGX Bootstrap to improve the user experience.

The original version of this application is contained in the DatingApp-SPA and DatingApp.API folders.
An updated version, using Identity and Role Management provided by .NET Core is contained in the Identity folder. This application also has an additional moderator feature where admins and moderators have to approve or reject photos that a user uploads. When a user uplaods a photo, it cannot be seen by any other users until it has been approved by an admin or a moderator.