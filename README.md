# 🚗 Car Stock API

A RESTful Web API built using C#, FastEndpoints, Dapper, and SQLite, that allows multiple dealers to securely manage their car inventory.

---

## Features

- Dealer registration and login
- JWT-based authentication
- Add car
- Get car by ID
- List all cars with stock levels
- Update car stock
- Search cars by make, model, and year
- Soft delete (cars are not permanently removed)

---

## Authentication & Authorization

The API uses **JWT (JSON Web Token)** for authentication.

### Flow:
1. Dealer registers
2. Dealer logs in
3. API generates a JWT token
4. Token contains `DealerId`
5. Client sends token with every request

### Header Example:
```
Authorization: Bearer <your_token>
```

### Data Isolation
- Each dealer can only access their own cars
- All database queries filter using `DealerId`
- Prevents access to other dealers' data

---

## Tech Stack

- C#
- FastEndpoints
- Dapper
- SQLite
- Swagger (NSwag)

---

## How to Run

1. Open solution in Visual Studio  
2. Press **F5**  
3. Open:

```
https://localhost:7200/swagger
```

---

## Testing Steps

1. Register → `/auth/register`  
2. Login → `/auth/login`  
3. Copy token  
4. Click **Authorize**  
5. Enter:

```
Bearer <token>
```

---

## Endpoints

### Auth
- POST `/auth/register`
- POST `/auth/login`

### Cars
- POST `/cars`
- GET `/cars`
- GET `/cars/{CarId}`
- GET `/cars/search`
- PUT `/cars/{CarId}/stock`
- DELETE `/cars/{CarId}`

---

## Important Notes

### SQLite Type Handling
- GUID stored as TEXT → manually converted
- INT returned as long → cast to int

### Soft Delete
- Uses `IsDeleted = 1`
- Queries filter `IsDeleted = 0`

---

## Submission Notes

- The project uses SQLite, so no external database setup is required.
- The database file (.db) is included in the repository.
- All APIs can be tested using Swagger UI.

### Please note:
- Do not include the following folders when submitting:
  - /bin
  - /obj
  - /.vs

---

## Author

Backend API built using FastEndpoints and Dapper.
