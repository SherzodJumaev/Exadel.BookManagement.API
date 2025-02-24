# Exadel.BookManagement.API
Exadel Book Management API is a .NET 8.0 Web API that provides functionality for managing books, users, and authentication. It is built using ASP.NET Core, Entity Framework Core, and JWT authentication.
# Feautures
- User authentication with JWT
- CRUD operations for books
- Entity Framework Core with SQL Server
- Swagger documentation for API endpoints
# Technologies Used
- .NET 8.0
- ASP.NET Core
- Entity Framework Core
- SQL Server
- JWT Authentication
- Swagger
# Prerequisites
- .NET 8.0 SDK
- SQL Server
# Steps
1. Clone the repository:

*https*
```md
git clone https://github.com/SherzodJumaev/Exadel.BookManagement.API.git
cd Exadel.BookManagement.API
```
*SSH*
```md
git clone git@github.com:SherzodJumaev/Exadel.BookManagement.API.git
cd Exadel.BookManagement.API
```
2. Set up the database connection in appsettings.json:
```json
"ConnectionStrings": {
  "DefaultConnection": "your-sql-server-connection-string"
}
```
3. Apply database migrations:
```md
dotnet ef database update
```
4. Run the application:
```md
dotnet run
```
# API Endpoints
**Authentication**
- POST /api/auth/register - Register a new user
- POST /api/auth/login - Authenticate a user and generate a JWT

**Books**
- GET /api/books - Retrieve all books (only titles)
- POST /api/Books - Create a new book
- GET /api/Books/{title} - Retrieve a book by title
- PUT /api/Books/{title} - Update an existing book by title
- DELETE /api/Books/{title} - Delete a book by title
- GET /api/Books/soft-deleted-books-titles - Retrieve soft-deleted book titles
- POST /api/Books/bulk - Bulk create books
- DELETE /api/Books/bulk - Bulk delete books
- PUT /api/Books/restore/{title} - Restore a soft-deleted book
