# Idear

## Description
An ASP.NET Core MVC project to implement a secure web-enabled role-based system
for collecting ideas for improvement from staff in a large University. The name
*Idear* comes from *Idea* and *Dear*, which means treasuring the ideas that the
staffs post.

## Run the project locally
1. Clone the project with
```shell
    git clone git@github.com:huynhloc-1110/Idear.git
```

2. Open `Idear.sln` with `Visual Studio` and wait a little for it to install
the necessary nuget packages.

3. Use Nuget Packet Manager Console to update your SQLServer LocalDB according
to the Migration Schema:
```shell
    Update-Database
```

3. Run the Project.