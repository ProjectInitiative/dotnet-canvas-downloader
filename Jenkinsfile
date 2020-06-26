pipeline {
    agent none
    stages {
        stage('Build on Linux') {
            agent { 
                label 'linux'
            }
            steps { 
                bash '$ENV:WORKSPACE/build.sh'
            }
        }
    }
}