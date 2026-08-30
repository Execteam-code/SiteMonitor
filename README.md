# Site Monitor
Web-панель для мониторинга и управления, построенная 
на базе ASP.NET Core MVC. Проект спроектирован для работы в 
среде Linux за reverse-прокси сервером (Nginx) и использует Entity 
Framework Core для работы с базой данных.
## 🛠 Стек технологий
* **Платформа:** .NET 8.0 * **Фреймворк:** ASP.NET Core MVC * **ORM:** Entity 
Framework Core * **Фронтенд:** Bootstrap 5, Bootstrap Icons * **Среда 
развертывания:** Linux (Kestrel + Nginx + Systemd)
## ✨ Основные возможности
* **Безопасность:** Cookie-авторизация и защита 
контроллеров от несанкционированного доступа. 
Элементы интерфейса скрыты за экраном входа 
администратора. * **Проксирование:** Встроенная 
поддержка заголовков `X-Forwarded-For` и `X-Forwarded-Proto` для 
корректной маршрутизации и определения IP при 
работе за Nginx. * **База данных:** Поддержка MS SQL Server с 
готовыми миграциями для быстрого развертывания 
структуры БД.
## 🚀 Быстрый старт (Локальная разработка)
### Требования
* [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) * MS SQL Server 
(рекомендуется запуск через Docker для Linux)
### Установка и запуск
1. Клонируйте репозиторий: ```bash git clone 
   [https://github.com/Execteam-code/SiteMonitor.git](https://github.com/Execteam-code/SiteMonitor.git) 
   cd SiteMonitor ```
2. Обновите строку подключения к базе данных в 
`appsettings.json`:
   ```json "ConnectionStrings": { "DefaultConnection": "Server=127.0.0.1,1433;Database=SiteMonitorDb;User 
       Id=sa;Password=YourPassword_123;TrustServerCertificate=True"
   }
   ``` 3. Примените миграции для создания структуры 
базы данных:
   ```bash dotnet ef database update ``` 4. Запустите проект: ```bash dotnet run --urls 
   "[http://0.0.0.0:5000](http://0.0.0.0:5000)" ```
## 📦 Развертывание в Production (Linux)
### 1. Сборка релиза
Скомпилируйте проект в директорию для публикации: 
```bash dotnet publish -c Release -o /var/www/sitemonitor ```
### 2. Настройка Systemd
Для фоновой работы приложения создайте сервис 
`sitemonitor.service` в `/etc/systemd/system/`: ```ini [Unit] Description=Site Monitor ASP.NET Core Web App 
After=network.target [Service] WorkingDirectory=/var/www/sitemonitor ExecStart=/usr/bin/dotnet 
/var/www/sitemonitor/SiteMonitor.dll --urls "[http://0.0.0.0:5000](http://0.0.0.0:5000)" Restart=always 
RestartSec=10 KillSignal=SIGINT SyslogIdentifier=sitemonitor User=root 
Environment=ASPNETCORE_ENVIRONMENT=Production [Install] WantedBy=multi-user.target ``` 
Запустите сервис: ```bash systemctl daemon-reload systemctl enable sitemonitor.service 
systemctl start sitemonitor.service ```
### 3. Настройка Nginx
Пример конфигурации `location` для проксирования 
трафика на приложение: ```nginx location / {
    proxy_pass [http://127.0.0.1:5000](http://127.0.0.1:5000); proxy_http_version 1.1; proxy_set_header 
    Upgrade $http_upgrade; proxy_set_header Connection keep-alive; proxy_set_header Host $host; 
    proxy_cache_bypass $http_upgrade; proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for; 
    proxy_set_header X-Forwarded-Proto $scheme;
}
```
