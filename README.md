# MLFoodAnalyzer

**MLFoodAnalyzer** — это открытый проект, разработанный с использованием .NET MAUI, предназначенный для анализа пищевых продуктов с применением методов машинного обучения.

## Описание

MLFoodAnalyzer предоставляет пользователям возможность анализировать пищевые продукты, используя современные технологии машинного обучения. Приложение разработано с использованием .NET MAUI, что обеспечивает кроссплатформенную поддержку и современный интерфейс.

## Функциональные возможности

- **Анализ пищевых продуктов**: Определение состава и питательной ценности продуктов.
- **Рекомендации по питанию**: Предоставление рекомендаций на основе анализа.
- **История анализов**: Хранение и просмотр предыдущих результатов.

## Структура проекта

Проект состоит из следующих основных компонентов:

- **Server/**: Серверная часть приложения, обрабатывающая запросы и выполняющая анализ данных.
- **Client/**: Клиентская часть приложения с пользовательским интерфейсом для взаимодействия с сервером.
- **.github/workflows/**: Конфигурации для GitHub Actions, обеспечивающие автоматизацию процессов CI/CD.

## Установка

Для запуска проекта локально выполните следующие шаги:

1. **Клонируйте репозиторий**:

   ```bash
   git clone https://github.com/DKAVrZoV65F/ml-food-analyzer.git
   ```

2. **Перейдите в директорию проекта**:

   ```bash
   cd ml-food-analyzer
   ```

3. **Соберите серверную часть**:

   ```bash
   cd Server
   dotnet build
   ```

4. **Соберите клиентскую часть**:

   ```bash
   cd ../Client
   dotnet build
   ```

5. **Запустите сервер**:

   ```bash
   cd ../Server
   dotnet run
   ```

6. **Запустите клиент**:

   ```bash
   cd ../Client
   dotnet run
   ```

## Использование

После запуска приложения вы сможете:

- **Анализировать продукты**: Введите данные о продукте для получения анализа.
- **Просматривать историю**: Получить доступ к предыдущим анализам.

## CI/CD

Проект использует GitHub Actions для автоматизации процессов непрерывной интеграции и доставки. Конфигурации находятся в директории `.github/workflows/`. Каждый push в репозиторий инициирует сборку и тестирование проекта, обеспечивая стабильность и качество кода.

## Вклад в проект

Мы приветствуем вклад сообщества! Если вы хотите внести свой вклад:

1. **Сделайте форк репозитория**.
2. **Создайте ветку** для вашей функции или исправления:

   ```bash
   git checkout -b feature/YourFeatureName
   ```

3. **Внесите изменения** и **зафиксируйте их**:

   ```bash
   git commit -m 'Добавлена новая функция: YourFeatureName'
   ```

4. **Отправьте изменения** в ваш форк:

   ```bash
   git push origin feature/YourFeatureName
   ```

5. **Создайте Pull Request** через GitHub.

## Лицензия

Этот проект лицензируется на условиях лицензии MIT. Подробности см. в файле [LICENSE](LICENSE).

