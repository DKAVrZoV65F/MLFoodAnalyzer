<h1 align="center">
    <img src="https://readme-typing-svg.herokuapp.com/?font=Righteous&size=35&center=true&vCenter=true&width=500&height=70&duration=5000&color=F7F7F7FF&background=00000055&lines=MLFoodAnalyzer;" />
</h1>

<h1 align="center">
  
  Открытый исходный код, .NET MAUI.<br/>
  **Русский** · [English](./README.md) · [Сообщить об ошибке](https://github.com/DKAVrZoV65F/MLFoodAnalyzer/issues) · [Предложить идею](https://github.com/DKAVrZoV65F/MLFoodAnalyzer/issues)
  
  <!-- SHIELD GROUP -->
  ![https://github.com/DKAVrZoV65F/MLFoodAnalyzer/releases](https://img.shields.io/github/v/release/DKAVrZoV65F/MLFoodAnalyzer?color=369eff&labelColor=black&logo=github&style=flat-square)
  ![https://github.com/DKAVrZoV65F/MLFoodAnalyzer/releases](https://img.shields.io/github/release-date/DKAVrZoV65F/MLFoodAnalyzer?labelColor=black&style=flat-square)
  ![https://github.com/DKAVrZoV65F/MLFoodAnalyzer/graphs/contributors](https://img.shields.io/github/contributors/DKAVrZoV65F/MLFoodAnalyzer?color=c4f042&labelColor=black&style=flat-square)
  ![https://github.com/DKAVrZoV65F/MLFoodAnalyzer/network/members](https://img.shields.io/github/forks/DKAVrZoV65F/MLFoodAnalyzer?color=8ae8ff&labelColor=black&style=flat-square)
  ![https://github.com/DKAVrZoV65F/MLFoodAnalyzer/network/stargazers](https://img.shields.io/github/stars/DKAVrZoV65F/MLFoodAnalyzer?color=ffcb47&labelColor=black&style=flat-square)
  ![https://github.com/DKAVrZoV65F/MLFoodAnalyzer/issues](https://img.shields.io/github/issues/DKAVrZoV65F/MLFoodAnalyzer?color=ff80eb&labelColor=black&style=flat-square)
  ![https://github.com/DKAVrZoV65F/MLFoodAnalyzer/blob/main/LICENSE](https://img.shields.io/github/license/DKAVrZoV65F/MLFoodAnalyzer?color=white&labelColor=black&style=flat-square)
</h1>



# Описание
MLFoodAnalyzer - программа, которая выдаёт информацию, какие овощи или фрукты несут пользу или вред здоровью.
Используется машинное обучение для распознования текста и картинок с помощью технологии ML.NET.
Написана программа на языке C#, где серверная часть - консольное приложение, а клиентская часть - мобильное или десктопное приложение написанная на фреймворке .NET MAUI.

# Эксплуатация приложения для клиенсткой части
1. Установить приложение на Windows/MacOS/Android, подключитесь к серверу через "Настройки" -> "Сеть", указав "IP" и "Порт".
2. После успешного подключения к серверу, можно отправлять текст с фруктами или овощами или отправить картинку.

# Эксплуатация приложения для серверной части
1. Установите Microsoft SQL Server 2019, запустите скрипт для добавления необходимых таблиц и полей.
2. Установить последней версии драйвера на видеокарту.
3. Распакуйте архив.
4. Запустите "Server".

# Требования
Сервер:
1. Microsoft SQL Server 2019
2. Драйверы для видеокарты

Клиент:
1. Разрешить доступ к камере и хранилищу данных

# Тестирование
· Android 12 - Клиент<br/>
· MacOS 14 - Клиент<br/>
· Windows 11 - Клиент/Сервер(с Nvidea)<br/>
· Linux (Ubuntu) - Сервер(с Nvidea)<br/>
