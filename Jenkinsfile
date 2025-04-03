pipeline {
    agent any

    environment {
        // Nom de l'image Docker
        DOCKER_IMAGE = "orthohelper-api:latest"
        // Ports exposés (doivent correspondre à votre Dockerfile)
        APP_PORT = "8088"
        // Configuration de build .NET
        BUILD_CONFIGURATION = "Release"
    }

    stages {
        // Étape 1 : Checkout du code depuis GitHub
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        // Étape 2 : Restauration des dépendances .NET
        stage('Restore') {
            steps {
                bat 'dotnet restore OrthoHelperAPI/OrthoHelperAPI.csproj'
            }
        }

        // Étape 3 : Build du projet
        stage('Build') {
            steps {
                bat "dotnet build OrthoHelperAPI/OrthoHelperAPI.csproj -c ${BUILD_CONFIGURATION} --no-restore"
            }
        }

        // Étape 4 : Exécution des tests (ajustez si vous avez des tests)
       
       //TODO AJOUTER ICI LES AUTRES PROJETS DE TESTS
       //stage('Test') {
       //     steps {
       //         bat '''
       //             echo "Lancement des tests..."
       //             dotnet test "OrthoHelper.Api.Controllers.Tests\\OrthoHelper.Api.Controllers.Tests.csproj" --no-build --verbosity normal
       //             dotnet test "OrthoHelper.Application.Tests\\OrthoHelper.Application.Tests.csproj" --no-build --verbosity normal
       //             dotnet test "OrthoHelper.Domain.Tests\\OrthoHelper.Domain.Tests.csproj" --no-build --verbosity normal
       //             dotnet test "OrthoHelper.Infrastructure.Tests\\OrthoHelper.Infrastructure.Tests.csproj" --no-build --verbosity normal
       //             dotnet test "OrthoHelper.Integration.Tests\\OrthoHelper.Integration.Tests.csproj" --no-build --verbosity normal
       //             dotnet test "OrthoHelperAPI.Tests\\OrthoHelperAPI.Tests.csproj" --no-build --verbosity normal
       //         '''
       //     }
       //     post {
       //         always {
       //             // Archive des résultats au format TRX (optionnel)
       //             archiveArtifacts artifacts: '**/TestResults/*.trx', allowEmptyArchive: true
       //         }
       //     }
       // }

        // Étape 5 : Publication de l'application
        stage('Publish') {
            steps {
                bat "dotnet publish OrthoHelperAPI/OrthoHelperAPI.csproj -c ${BUILD_CONFIGURATION} -o ./publibat --no-build"
            }
        }

        // Étape 6 : Construction de l'image Docker
        stage('Build Docker Image') {
            steps {
                script {
                    // Supprime l'image existante pour éviter les conflits
                    bat "docker rmi ${DOCKER_IMAGE} --force || exit 0"
                    // Build de l'image
                    bat "docker build -t ${DOCKER_IMAGE} ."
                }
            }
        }

        // Étape 7 : Déploiement du conteneur
        stage('Deploy') {
            steps {
                script {
                    // Arrêt et suppression du conteneur existant (ignore les erreurs)
                    bat "docker stop orthohelper-api || exit 0"
                    bat "docker rm orthohelper-api || exit 0"
                    // Lancement du nouveau conteneur
                    bat "docker run -d --name orthohelper-api -p ${APP_PORT}:${APP_PORT} ${DOCKER_IMAGE}"
                }
            }
        }
    }
    // Actions post-build (notifications, nettoyage)
    post {
        success {
            echo 'Build et déploiement réussis !'
            // Slack/Email notification optionnelle
        }
        failure {
            echo 'Échec du pipeline. Consultez les logs.'
        }
    }
}