pipeline {
    agent { node { label 'Debian10-Node' } }
    stages {
        stage('Build') {
            steps { 
                sh "${env.WORKSPACE}/build.sh"
            }
        }
    }
    post {
        success {
                 // we only worry about archiving the files if the build steps are successful
                archiveArtifacts(artifacts: 'bin/artifacts/', allowEmptyArchive: true)
            }
        always {
                cleanWs()
            }
         }

}