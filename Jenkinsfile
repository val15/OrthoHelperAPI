pipeline {
    agent any

    environment {
        DOCKER_IMAGE = "orthohelper-api:latest"
        APP_PORT = "7088"
        BUILD_CONFIGURATION = "Release"
        DB_VOLUME = "orthohelper-data" // Volume pour la base de donn�es
    }

    stages {
        // �tape 1 : R�cup�ration du code
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        // �tape 2 : Restauration des d�pendances
        stage('Restore') {
            steps {
                bat 'dotnet restore OrthoHelperAPI/OrthoHelperAPI.csproj'
            }
        }

        // �tape 3 : Compilation
        stage('Build') {
            steps {
                bat "dotnet build OrthoHelperAPI/OrthoHelperAPI.csproj -c ${BUILD_CONFIGURATION} --no-restore"
            }
        }

        // �tape 4 : Ex�cution de tous les tests
        //stage('Test') {
        //    steps {
        //        bat '''
        //            echo "Lancement des tests..."
        //            dotnet test "OrthoHelper.Api.Controllers.Tests\\OrthoHelper.Api.Controllers.Tests.csproj" --no-build --verbosity normal
        //            dotnet test "OrthoHelper.Application.Tests\\OrthoHelper.Application.Tests.csproj" --no-build --verbosity normal
        //            dotnet test "OrthoHelper.Domain.Tests\\OrthoHelper.Domain.Tests.csproj" --no-build --verbosity normal
        //            dotnet test "OrthoHelper.Infrastructure.Tests\\OrthoHelper.Infrastructure.Tests.csproj" --no-build --verbosity normal
        //            dotnet test "OrthoHelper.Integration.Tests\\OrthoHelper.Integration.Tests.csproj" --no-build --verbosity normal
        //            dotnet test "OrthoHelperAPI.Tests\\OrthoHelperAPI.Tests.csproj" --no-build --verbosity normal
        //        '''
        //    }
        //    post {
        //        always {
        //            archiveArtifacts artifacts: '**/TestResults/*.trx', allowEmptyArchive: true
        //        }
        //    }
        //}

        // �tape 5 : Publication
        stage('Publish') {
            steps {
                bat "dotnet publish OrthoHelperAPI/OrthoHelperAPI.csproj -c ${BUILD_CONFIGURATION} -o ./publish --no-build"
            }
        }

        // �tape 6 : Construction de l'image Docker
        stage('Build Docker Image') {
            steps {
                script {
                    bat "docker rmi ${DOCKER_IMAGE} --force || exit 0"
                    bat "docker build -t ${DOCKER_IMAGE} ."
                }
            }
        }

        // �tape 7 : D�ploiement avec volume persistant
        stage('Deploy') {
            steps {
                script {
                    // Nettoyage des anciens conteneurs
                    bat "docker stop orthohelper-api || exit 0"
                    bat "docker rm orthohelper-api || exit 0"
                    
                    // Lancement avec volume et variables d'environnement
                    bat """
                        docker run -d \
                        --name orthohelper-api \
                        -p ${APP_PORT}:8080 \
                        -v ${DB_VOLUME}:/app/data \
                        -e ASPNETCORE_ENVIRONMENT=Production \
                        -e CONNECTIONSTRINGS__DEFAULTCONNECTION="Data Source=/app/data/api.db;" \
                        ${DOCKER_IMAGE}
                    """
                }
            }
        }
    }

    // Notifications
    post {
        success {
            echo '? D�ploiement r�ussi! Acc�dez � : http://localhost:8088/swagger'
        }
        failure {
            echo '? �chec du pipeline. V�rifiez les logs Jenkins.'
        }
    }
}