pipeline {
    agent none
    stages {
        stage('Build') {
            agent { 
                label 'Debian10-Node'
            }
            steps { 
                sh '$ENV:WORKSPACE/build.sh'
            }
        }
    }
    post {
        success {
          // we only worry about archiving the files if the build steps are successful
          archiveArtifacts(artifacts: '**/publish/*', allowEmptyArchive: true)
        }
      }
}