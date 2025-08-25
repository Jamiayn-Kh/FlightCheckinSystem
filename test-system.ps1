# Flight Check-in System Test Script
# This script tests the basic functionality of the system

Write-Host "Flight Check-in System Test Script" -ForegroundColor Green
Write-Host "=================================" -ForegroundColor Green

# Test 1: Check if server is running
Write-Host "`n1. Testing server connectivity..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "http://localhost:5000/api/flight" -Method Get -TimeoutSec 5
    Write-Host "✓ Server is running and responding" -ForegroundColor Green
    Write-Host "  Found $($response.Count) flights" -ForegroundColor Gray
} catch {
    Write-Host "✗ Server is not running or not accessible" -ForegroundColor Red
    Write-Host "  Please start the server first: cd FlightCheckin.Server && dotnet run" -ForegroundColor Gray
    exit 1
}

# Test 2: Test flight status change
Write-Host "`n2. Testing flight status change..." -ForegroundColor Yellow
try {
    $statusRequest = @{
        FlightNumber = "MGL101"
        Status = 1  # Boarding
    } | ConvertTo-Json

    $response = Invoke-RestMethod -Uri "http://localhost:5000/api/flight/status" -Method Put -Body $statusRequest -ContentType "application/json"
    Write-Host "✓ Flight status changed successfully" -ForegroundColor Green
    Write-Host "  Flight: $($response.FlightNumber), Status: $($response.Status)" -ForegroundColor Gray
} catch {
    Write-Host "✗ Failed to change flight status" -ForegroundColor Red
    Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Gray
}

# Test 3: Test passenger check-in
Write-Host "`n3. Testing passenger check-in..." -ForegroundColor Yellow
try {
    $checkinRequest = @{
        FlightNumber = "MGL101"
        PassportNumber = "TEST001"
        PassengerName = "Test Passenger"
    } | ConvertTo-Json

    $response = Invoke-RestMethod -Uri "http://localhost:5000/api/checkin" -Method Post -Body $checkinRequest -ContentType "application/json"
    Write-Host "✓ Passenger check-in successful" -ForegroundColor Green
    Write-Host "  Seat assigned: $($response.SeatCode)" -ForegroundColor Gray
} catch {
    Write-Host "✗ Failed to check in passenger" -ForegroundColor Red
    Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Gray
}

# Test 4: Test seat availability
Write-Host "`n4. Testing seat availability..." -ForegroundColor Yellow
try {
    $seats = Invoke-RestMethod -Uri "http://localhost:5000/api/checkin/seats/MGL101" -Method Get
    $availableSeats = ($seats | Where-Object { -not $_.IsTaken }).Count
    $takenSeats = ($seats | Where-Object { $_.IsTaken }).Count
    Write-Host "✓ Seat availability retrieved" -ForegroundColor Green
    Write-Host "  Available seats: $availableSeats" -ForegroundColor Gray
    Write-Host "  Taken seats: $takenSeats" -ForegroundColor Gray
} catch {
    Write-Host "✗ Failed to get seat availability" -ForegroundColor Red
    Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Gray
}

# Test 5: Test concurrent booking prevention
Write-Host "`n5. Testing concurrent booking prevention..." -ForegroundColor Yellow
try {
    $tasks = @()
    $results = @()
    
    # Create 3 concurrent booking requests for the same seat
    for ($i = 1; $i -le 3; $i++) {
        $checkinRequest = @{
            FlightNumber = "MGL101"
            PassportNumber = "CONCURRENT$i"
            PassengerName = "Concurrent Passenger $i"
            SeatRow = 2
            SeatColumn = "A"
        } | ConvertTo-Json

        $task = Start-Job -ScriptBlock {
            param($uri, $body)
            try {
                $response = Invoke-RestMethod -Uri $uri -Method Post -Body $body -ContentType "application/json"
                return @{ Success = $true; Response = $response }
            } catch {
                return @{ Success = $false; Error = $_.Exception.Message }
            }
        } -ArgumentList "http://localhost:5000/api/checkin", $checkinRequest
        
        $tasks += $task
    }

    # Wait for all tasks to complete
    Wait-Job $tasks | Out-Null
    
    # Collect results
    foreach ($task in $tasks) {
        $result = Receive-Job $task
        $results += $result
        Remove-Job $task
    }

    $successCount = ($results | Where-Object { $_.Success }).Count
    Write-Host "✓ Concurrent booking test completed" -ForegroundColor Green
    Write-Host "  Successful bookings: $successCount out of 3" -ForegroundColor Gray
    Write-Host "  Expected: 1 successful booking (concurrent prevention working)" -ForegroundColor Gray
    
    if ($successCount -eq 1) {
        Write-Host "  ✓ Concurrent booking prevention is working correctly" -ForegroundColor Green
    } else {
        Write-Host "  ⚠ Concurrent booking prevention may not be working as expected" -ForegroundColor Yellow
    }
} catch {
    Write-Host "✗ Failed to test concurrent booking" -ForegroundColor Red
    Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Gray
}

Write-Host "`nTest completed!" -ForegroundColor Green
Write-Host "To run the full system:" -ForegroundColor Cyan
Write-Host "1. Start server: cd FlightCheckin.Server && dotnet run" -ForegroundColor Gray
Write-Host "2. Start desktop app: cd FlightCheckin.Desktop && dotnet run" -ForegroundColor Gray
Write-Host "3. Start web app: cd FlightCheckin.Web && dotnet run" -ForegroundColor Gray



