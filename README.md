# ДЖАРВИС FIRE Windows — стартовый каркас

**Статус: прототип. Не версия 1.0.0.0; не использовать в реальной диспетчерской работе.**

## Состав
- `src/JarvisFire.Desktop`: минимальный интерфейс WPF / .NET 8.
- `database/001_initial.sql`: черновая схема PostgreSQL (ещё не подключена к приложению).
- `database/docker-compose.yml`: локальная PostgreSQL.
- `.github/workflows/windows.yml`: сборка Windows x64 в GitHub Actions.

## Запуск
На Windows 10/11 с .NET 8 SDK: `dotnet run --project src/JarvisFire.Desktop`.

## Не реализовано
Авторизация, вызовы, карта, GPS, статусы техники, обмен с Android, импорт Excel, Word, API, тестирование и установщик EXE.

## Следующий этап
Проверить успешную сборку Actions, затем реализовать функции по согласованному ТЗ (16 этапов / 80 вопросов).