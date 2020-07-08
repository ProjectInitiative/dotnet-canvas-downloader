pipeline {
    agent none
    stages {
        stage('Build on Linux') {
            agent { 
                label 'Debian10-Node'
            }
            steps { 
                sh '$ENV:WORKSPACE/build.sh'
            }
        }
        stage('Archive Artifacts') {

        }
    }
}