# Library System - Team 07

This project is a simple library system.  
It has CRUD operations (Create, Read, Update, Delete) for **Books**, **Users**, and **Loans**.  
Each entity has REST endpoints.

## Team Members
- Soria Aguirre Rene
- Maiz Hinojosa Juan Pablo
- Rimassa Fernandez Ernesto David

## Endpoints (Team 07)

### Books
| Method | Endpoint          | Description             |
|--------|------------------|-------------------------|
| GET    | /api/books       | List all books          |
| GET    | /api/books/{id}  | Get book by ID          |
| POST   | /api/books       | Add new book            |
| PUT    | /api/books/{id}  | Update book by ID       |
| PATCH  | /api/books/{id}  | Partially update a book |
| DELETE | /api/books/{id}  | Delete book by ID       |

### Users
| Method | Endpoint          | Description             |
|--------|------------------|-------------------------|
| GET    | /api/users       | List all users          |
| GET    | /api/users/{id}  | Get user by ID          |
| POST   | /api/users       | Add new user            |
| PUT    | /api/users/{id}  | Update user by ID       |
| PATCH  | /api/users/{id}  | Partially update a user |
| DELETE | /api/users/{id}  | Delete user by ID       |

### Loans
| Method | Endpoint          | Description          |
|--------|------------------|----------------------|
| GET    | /api/loans       | List all loans       |
| GET    | /api/loans/{id}  | Get loan by ID       |
| POST   | /api/loans       | Add new loan         |
| PUT    | /api/loans/{id}  | Update loan by ID    |
| DELETE | /api/loans/{id}  | Delete loan by ID    |
