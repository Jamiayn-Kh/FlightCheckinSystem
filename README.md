# Flight Check-in System (Онгоцны зорчигч бүртгэл, мэдээлэх систем)

A comprehensive flight check-in system built with .NET 8, featuring real-time updates, concurrent booking prevention, and multiple client applications.

## System Overview

This system allows airline staff to check in passengers and manage flight information with real-time updates displayed on information boards. The system prevents concurrent seat booking conflicts and provides both desktop and web interfaces.

## Architecture

The system follows a layered architecture with the following components:

- **FlightCheckin.Models**: Data models and DTOs
- **FlightCheckin.DataAccess**: Database context and repositories
- **FlightCheckin.BusinessLogic**: Business services and validation
- **FlightCheckin.Server**: Web API server with SignalR and Socket server
- **FlightCheckin.Desktop**: Windows Forms application for check-in staff
- **FlightCheckin.Web**: Blazor web application for flight information display

## Features

### Core Functionality
- ✅ Passenger check-in by passport number
- ✅ Automatic or manual seat assignment
- ✅ Boarding pass generation
- ✅ Flight status management
- ✅ Real-time seat availability updates

### Technical Features
- ✅ Pessimistic locking to prevent concurrent seat booking
- ✅ Real-time updates using SignalR
- ✅ Socket server for TCP communication
- ✅ REST API for HTTP communication
- ✅ SQLite database with Entity Framework Core
- ✅ Concurrent booking simulation and testing

### Client Applications
- ✅ Windows Forms desktop application for check-in staff
- ✅ Blazor web application for flight information display
- ✅ Concurrent booking test application

## Flight Statuses

The system supports the following flight statuses:
- **CheckingIn** (Бүртгэж байна)
- **Boarding** (Онгоцонд сууж байна)
- **Departed** (Ниссэн)
- **Delayed** (Хойшилсон)
- **Cancelled** (Цуцалсан)

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code
- SQLite (included with Entity Framework)

### Running the Application

1. **Start the Server**:
   ```bash
   cd FlightCheckin.Server
   dotnet run
   ```
   The server will start on `http://localhost:5000` with:
   - REST API endpoints
   - SignalR hub on `/flightStatusHub`
   - Socket server on port 8888
   - Swagger UI on `/swagger`

2. **Start the Desktop Application**:
   ```bash
   cd FlightCheckin.Desktop
   dotnet run
   ```
   Choose between:
   - Main Check-in Form: For regular passenger check-in
   - Concurrent Test Form: To test concurrent booking prevention

3. **Start the Web Application**:
   ```bash
   cd FlightCheckin.Web
   dotnet run
   ```
   Navigate to `http://localhost:5001` to view the flight information board.

### Database Setup

The system uses SQLite with Entity Framework Core. The database will be created automatically on first run with seed data including:
- Sample flight (MGL101 to Tokyo)
- 60 seats (10 rows × 6 columns A-F)

## API Endpoints

### Flight Management
- `GET /api/flight` - Get all flights
- `GET /api/flight/{flightNumber}` - Get specific flight
- `PUT /api/flight/status` - Change flight status

### Check-in Management
- `GET /api/checkin/seats/{flightNumber}` - Get seats for a flight
- `POST /api/checkin` - Check in a passenger

### Socket Server
- TCP server on port 8888
- Accepts JSON check-in requests
- Returns JSON responses

## Real-time Communication

### SignalR Hub
- **FlightStatusUpdated**: Broadcasts flight status changes
- **SeatAssigned**: Broadcasts seat assignments

### WebSocket Events
- Join flight groups for real-time updates
- Automatic reconnection handling

## Concurrent Booking Prevention

The system implements pessimistic locking using database transactions with `Serializable` isolation level to prevent:
- Multiple passengers booking the same seat
- Race conditions during seat assignment
- Data inconsistency

### Testing Concurrent Booking

Use the Concurrent Test Form to simulate multiple passengers trying to book the same seat simultaneously. The system will:
1. Allow only one passenger to successfully book the seat
2. Return appropriate error messages to other passengers
3. Maintain data consistency

## Project Structure

```
FlightCheckinSystem/
├── FlightCheckin.Models/           # Data models and DTOs
├── FlightCheckin.DataAccess/       # Database context and repositories
├── FlightCheckin.BusinessLogic/    # Business services and validation
├── FlightCheckin.Server/           # Web API server
├── FlightCheckin.Desktop/          # Windows Forms application
└── FlightCheckin.Web/              # Blazor web application
```

## Development

### Adding New Features
1. Add models in `FlightCheckin.Models`
2. Create repositories in `FlightCheckin.DataAccess`
3. Implement business logic in `FlightCheckin.BusinessLogic`
4. Add API endpoints in `FlightCheckin.Server`
5. Update client applications as needed

### Database Migrations
```bash
cd FlightCheckin.DataAccess
dotnet ef migrations add MigrationName
dotnet ef database update
```

## Troubleshooting

### Common Issues
1. **Port conflicts**: Ensure ports 5000, 5001, and 8888 are available
2. **Database issues**: Delete `flights.db` to reset the database
3. **SignalR connection**: Check if the server is running and accessible

### Logs
- Server logs are displayed in the console
- Desktop application shows status messages
- Web application logs to browser console

## Requirements Fulfillment

This system fulfills all the requirements specified in the project:

- ✅ UML diagrams (can be generated from code)
- ✅ Windows native app (WinForms)
- ✅ Passenger search by passport number
- ✅ Boarding pass generation
- ✅ Flight status management
- ✅ Socket server with Thread/Task
- ✅ REST API
- ✅ Data Access and Business Logic separation
- ✅ SignalR hub for real-time updates
- ✅ Real-time web application with SignalR
- ✅ Blazor web application
- ✅ SQLite database
- ✅ Concurrent booking prevention
- ✅ Real-time status updates

## License

This project is created for educational purposes as a team assignment.



