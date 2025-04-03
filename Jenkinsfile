pipeline {
    agent any

    environment {
        // Nom de l'image Docker
        DOCKER_IMAGE = "orthohelper-api:latest"
        // Ports expos�s (doivent correspondre � votre Dockerfile)
        APP_PORT = "8080"
        // Configuration de build .NET
        BUILD_CONFIGURATION = "Release"
    }

    stages {
        // �tape 1 : Checkout du code depuis GitHub
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        // �tape 2 : Restauration des d�pendances .NET
        stage('Restore') {
            steps {
                bat 'dotnet restore OrthoHelperAPI/OrthoHelperAPI.csproj'
            }
        }

        // �tape 3 : Build du projet
        stage('Build') {
            steps {
                bat "dotnet build OrthoHelperAPI/OrthoHelperAPI.csproj -c ${BUILD_CONFIGURATION} --no-restore"
            }
        }

        // �tape 4 : Ex�cution des tests (ajustez si vous avez des tests)
        stage('Test') {
            steps {
                bat '''
                    echo "Lancement de tous les projets de test..."
                    dotnet test **/*.Tests.csproj --no-build --verbosity normal --logger "trx;LogFileName=TestResults.trx"
                '''
            }
            post {
                always {
                    // Archive les r�sultats des tests pour inspection
                    archiveArtifacts artifacts: '**/TestResults/**/*', allowEmptyArchive: true
                }
            }
        }

        // �tape 5 : Publication de l'application
        stage('Publish') {
            steps {
                bat "dotnet publibat OrthoHelperAPI/OrthoHelperAPI.csproj -c ${BUILD_CONFIGURATION} -o ./publibat --no-build"
            }
        }

        // �tape 6 : Construction de l'image Docker
        stage('Build Docker Image') {
            steps {
                script {
                    // Supprime l'image existante pour �viter les conflits
                    bat "docker rmi ${DOCKER_IMAGE} --force || true"
                    // Build de l'image
                    bat "docker build -t ${DOCKER_IMAGE} ."
                }
            }
        }

        // �tape 7 : D�ploiement du conteneur
        stage('Deploy') {
            steps {
                script {
                    // Arr�t et suppression du conteneur existant (ignore les erreurs)
                    bat "docker stop orthohelper-api || true"
                    bat "docker rm orthohelper-api || true"
                    // Lancement du nouveau conteneur
                    bat "docker run -d --name orthohelper-api -p ${APP_PORT}:${APP_PORT} ${DOCKER_IMAGE}"
                }
            }
        }
    }
    // Actions post-build (notifications, nettoyage)
    post {
        success {
            echo 'Build et d�ploiement r�ussis !'
            // Slack/Email notification optionnelle
        }
        failure {
            echo '�chec du pipeline. Consultez les logs.'
        }
    }
}