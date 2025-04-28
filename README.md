# OrthoHelperAPI

OrthoHelperAPI is a French spelling corrector powered by AI. It supports both locally installed models (via Ollama) and external LLMs such as Gemmini Advance through API keys. This project includes a backend API built in C# and a frontend developed in Angular. Both applications, along with the database, are encapsulated in a single Docker image for streamlined deployment.

## Features
- Leverages AI for advanced text correction.
- Supports local AI models and external LLMs.
- Provides a RESTful API for text correction and user management.
- Integrated frontend and backend and database in a single Docker image.

## Endpoints

### AuthController
- **`POST /api/auth/register`**
  - Registers a new user.
- **`POST /api/auth/login`**
  - Logs in a user and returns an authentication token.



### TextCorrectionController
- **`POST /api/textcorrection/correct`**
  - Corrects text using AI.
- **`GET /api/textcorrection/messages`**
  - Browses the user's text correction sessions.
- **`DELETE /api/textcorrection/DeleteUserMessages`**
  - Deletes all user correction sessions.
- **`GET /api/textcorrection/Models`**
  - Lists all available AI models for text correction.


## Docker Instructions

### Build Docker Image
```bash
docker build -t orthohelperapi .
```

### Run Docker Container
#use docker compose up
#Exaple docker compose dontainte:
```bash
services:
  OrthoHelperAPI:
    image: ortho-helper-api:v0.0.test
    ports:
      - 7088:8080    
    volumes:
      - orthohelper-data:/app/data  # SQLite Volume 
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - CONNECTIONSTRINGS__DEFAULTCONNECTION=Data Source=/app/data/api.db;
      - GOOGLE_AI_GEMINI_API_KEY=YOUR_GOOGLE_AI_GEMINI_API_KEY
volumes:
  orthohelper-data:
```

This will expose the application on ports `7080`

## Getting Started

1. Clone the repository:
   ```bash
   git clone https://github.com/val15/OrthoHelperAPI.git
   ```
2. Navigate to the project directory:
   ```bash
   cd OrthoHelperAPI
   ```
3. Build and run the Docker container using the commands provided above.
4. Acces frontend app in http://localhost/7080

## Technologies Used
- **Backend**: C# with .NET
- **Frontend**: Angular
- **Containerization**: Docker
- **AI Models**: Ollama (local) and Gemmini Advance (external)
- **DataBase**: SQLite

## Contributing
Contributions are welcome! Please fork the repository and submit a pull request.

## License
This project is licensed under the MIT License.

---

For more details, visit the [repository](https://github.com/val15/OrthoHelperAPI).
