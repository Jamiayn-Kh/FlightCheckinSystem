## README (MN) — Онгоцны зорчигч бүртгэл, мэдээллийн систем

Тойм
- Зорчигчдыг бүртгэх, нислэгийн мэдээллийг real-time байдлаар дэлгэцэнд түгээх систем.
- Зэрэгцээ суудал захиалгын зөрчлийг (conflict) транзакц, unique индексээр хамгаална.
- Desktop (WinForms) болон Web (Blazor) интерфэйс, мөн TCP Socket клиентүүд дэмжинэ.

Архитектур
- FlightCheckin.Models: Entity/DTO моделууд
- FlightCheckin.DataAccess: EF Core контекст, репозитори
- FlightCheckin.BusinessLogic: Бизнес сервис, валидаци
- FlightCheckin.Server: Web API + SignalR + Socket сервер
- FlightCheckin.Desktop: WinForms бүртгэлийн апп
- FlightCheckin.Web: Blazor Server мэдээллийн дэлгэц

Гол боломжууд
- ✅ Паспортын дугаараар зорчигч бүртгэх/хайх
- ✅ Суудал автоматаар/гараар оноох
- ✅ Boarding pass хэвлэх (PrintPreview/PrintDocument)
- ✅ Нислэгийн төлөв солих
- ✅ Суудлын төлвийг real-time шинэчлэх (SignalR)

Техникийн боломжууд
- ✅ Зэрэгцээ захиалгыг транзакц (Serializable) + unique индексээр сэргийлэх
- ✅ Real-time (SignalR) эвентууд: FlightStatusUpdated, SeatAssigned
- ✅ TCP Socket сервер (порт 8888), JSON мөр-андсан протокол
- ✅ REST API (HTTP)
- ✅ SQLite + EF Core
- ✅ Зэрэгцээ захиалгын симуляци (ConcurrentTestForm)

Клиентууд
- ✅ WinForms (бүртгэл)
- ✅ Blazor Web (табло)
- ✅ Concurrent тест апп (симуляци)

Нислэгийн төлвүүд
- CheckingIn (Бүртгэж байна) / Boarding (Онгоцонд сууж байна) / Departed (Ниссэн) / Delayed (Хойшилсон) / Cancelled (Цуцалсан)

Ажиллуулах
1) Server
- cd FlightCheckin.Server
- dotnet run
- http://localhost:5002 (эсвэл таны порт)
- Swagger: /swagger, SignalR hub: /flightStatusHub, Socket: 8888

2) Desktop
- cd FlightCheckin.Desktop
- dotnet run
- Yes: Check-in Form, No: Concurrent Test

3) Web
- cd FlightCheckin.Web
- dotnet run
- https://localhost:7075 (Blazor dev default)

Өгөгдлийн сан
- SQLite автоматаар үүснэ (анхны seed: MGL101 + 10×6 суудал)

API (жишээ)
- GET /api/flight — нислэгүүд
- GET /api/flight/{flightNumber}
- PUT /api/flight/status — төлөв солих
- GET /api/checkin/seats/{flightNumber}
- POST /api/checkin — check-in

Socket сервер (TCP 8888)
- Нэг холболт = нэг JSON мөр хүсэлт; хариуд нэг JSON мөр буцаана
- Жишээ хүсэлт: {"FlightNumber":"MGL101","PassportNumber":"A1234567","PassengerName":"User","SeatRow":7,"SeatColumn":"A"}

Real-time (SignalR)
- FlightStatusUpdated — Төлөв өөрчлөгдсөн мэдээлэл
- SeatAssigned — Суудал оноогдсон мэдээлэл
- Вэб талд группт нэгдэж (JoinFlightGroup), автоматаар шинэчлэгдэнэ

Зэрэгцээ суудал сэргийлэлт
- Транзакц (Serializable) + unique индекс
- Зөвхөн нэг хүсэлт амжилттай; бусдад нь “already taken”

Симуляци (олон хүн ижил суудал)
- Desktop → ConcurrentTestForm: 5 зорчигчийг зэрэг ижил суудалд оролцуулж тестлэх

Төслийн бүтэц
- FlightCheckinSystem/
  - FlightCheckin.Models/
  - FlightCheckin.DataAccess/
  - FlightCheckin.BusinessLogic/
  - FlightCheckin.Server/
  - FlightCheckin.Desktop/
  - FlightCheckin.Web/

Хөгжүүлэлт
- Models → DataAccess → BusinessLogic → Server API → Client шинэчлэлт
- Миграци:
  - cd FlightCheckin.DataAccess
  - dotnet ef migrations add <Name>
  - dotnet ef database update

Саатал арилгах
- Порт зөрчил: 5002 (эсвэл таны тохируулсан) болон 8888 чөлөөтэй эсэх
- DB асуудал: flights.db устгаад сэргээх
- SignalR: Server ажиллаж байгаа эсэх, CORS/URL зөв эсэх

Логууд
- Server: Console
- Desktop: Status label/мессеж
- Web: Browser console

Шаардлагын биелэлт
- ✅ UML (Use Case, Activity, Sequence)
- ✅ Windows native app (WinForms)
- ✅ Паспортын дугаараар хайх
- ✅ Boarding pass хэвлэх
- ✅ Төлөв солих (real-time)
- ✅ Socket сервер (Task/BackgroundService)
- ✅ REST API
- ✅ DataAccess/BusinessLogic тусгаарласан
- ✅ SignalR hub (real-time)
- ✅ Blazor web app (табло)
- ✅ SQLite
- ✅ Concurrent сэргийлэлт
- ✅ Real-time шинэчлэлт

