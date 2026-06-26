# Cybersecurity Awareness Chatbot

## Overview
A WPF-based cybersecurity awareness chatbot that educates users about online safety through conversation, tasks, and quizzes.

## Features
- **Part 1**: Voice greeting, ASCII art, name personalisation, basic responses
- **Part 2**: GUI interface, keyword recognition, random responses, sentiment detection, user memory
- **Part 3**: Task manager with SQLite database, cybersecurity quiz with 12 questions, activity log, NLP simulation

## Prerequisites
- Visual Studio 2019 or later
- .NET Framework 4.7.2 or later
- SQLite (included via NuGet package - automatically installed on build)

## Installation
1. Clone this repository
2. Open `chatbot_ai.sln` in Visual Studio
3. Restore NuGet packages (right-click solution → Restore NuGet Packages)
4. Build the solution (Ctrl+Shift+B)
5. Run the application (F5)

## Database
- SQLite database file `chatbot_data.db` is created automatically on first run
- No manual database setup required

## Usage
1. Enter your name when prompted
2. Use the main menu to:
   - Ask cybersecurity questions
   - View chat history
   - Open Task Manager (Part 3)
   - Start Quiz (Part 3)
3. In conversation mode, ask about:
   - Password safety
   - Phishing
   - Scams
   - Privacy
   - Safe browsing

## NLP Commands (Part 3)
- `show activity log` - View recent activities
- `add task` - Open task manager
- `start quiz` - Open quiz
- `what have you done` - View activity log

## Video Presentation

## GitHub Repository
https://github.com/ST10493031/ai-chatbot-part2
## Date
June 2026
