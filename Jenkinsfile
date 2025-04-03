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
                sh 'dotnet restore OrthoHelperAPI.csproj'
            }
        }

        // �tape 3 : Build du projet
        stage('Build') {
            steps {
                sh "dotnet build OrthoHelperAPI.csproj -c ${BUILD_CONFIGURATION} --no-restore"
            }
        }

        // �tape 4 : Ex�cution des tests (ajustez si vous avez des tests)
        stage('Test') {
            steps {
                sh 'dotnet test --no-build --verbosity normal'
            }
        }

        // �tape 5 : Publication de l'application
        stage('Publish') {
            steps {
                sh "dotnet publish OrthoHelperAPI.csproj -c ${BUILD_CONFIGURATION} -o ./publish --no-build"
            }
        }

        // �tape 6 : Construction de l'image Docker
        stage('Build Docker Image') {
            steps {
                script {
                    // Supprime l'image existante pour �viter les conflits
                    sh "docker rmi ${DOCKER_IMAGE} --force || true"
                    // Build de l'image
                    sh "docker build -t ${DOCKER_IMAGE} ."
                }
            }
        }

        // �tape 7 : D�ploiement du conteneur
        stage('Deploy') {
            steps {
                script {
                    // Arr�t et suppression du conteneur existant (ignore les erreurs)
                    sh "docker stop orthohelper-api || true"
                    sh "docker rm orthohelper-api || true"
                    // Lancement du nouveau conteneur
                    sh """
                        docker run -d \
                        --name orthohelper-api \
                        -p ${APP_PORT}:${APP_PORT} \
                        ${DOCKER_IMAGE}
                    """
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