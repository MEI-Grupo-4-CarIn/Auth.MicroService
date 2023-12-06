# Kubernetes Deployment

This section provides instructions on how to deploy the Auth.MicroService to a Kubernetes cluster.

## Prerequisites

Before you can deploy this project to a Kubernetes cluster, you need to have the following installed on your machine:

- Kubernetes CLI (kubectl)
- Minikube (for local testing)

## Installing

1. Clone the repository to your local machine.
2. If you're using Minikube, start your local cluster by running the command `minikube start`.
3. Navigate to the Kubernetes directory in the repository.
4. Create a Secret for the SMTP password:
  ```bash
  kubectl create secret generic smtp-password --from-literal=SMTP_PASSWORD=your_smtp_password
  ```
  Replace `your_smtp_password` with the actual SMTP password.

5. Run the following commands (**in this order**) to deploy the Auth.MicroService and its dependencies to your Kubernetes cluster:
  ```bash
  kubectl apply -f elasticsearch-config.yml
  kubectl apply -f elasticsearch-deployment.yml
  kubectl apply -f elasticsearch-service.yml
  kubectl apply -f sql-server-deployment.yml
  kubectl apply -f sql-server-service.yml
  kubectl apply -f kibana-config.yml
  kubectl apply -f kibana-deployment.yml
  kubectl apply -f kibana-service.yml
  kubectl apply -f auth-microservice-deployment.yml
  kubectl apply -f auth-microservice-service.yml
  ```

## Verifying the Deployment

After deploying the Auth.MicroService to your Kubernetes cluster, you can verify that it is running correctly by running the following command:
```bash
kubectl get pods
```
This command will list all the pods in your Kubernetes cluster. You should see all the pods for the Auth.MicroService with a status of `Running`.

## Testing (using Minikube)

If you're using Minikube on your machine, you can access the microservice and use it by running the following command:
```bash
minikube service auth-microservice
```
This command will create a `minikube tunnel` to access a LoadBalencer and open your browser on a generated port.

Also, if you wanna check the logs, you can run this and check the Kibana:
```bash
minikube service kibana
```
