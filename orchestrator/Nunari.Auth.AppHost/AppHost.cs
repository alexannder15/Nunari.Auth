var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgWeb();

var authdb = postgres.AddDatabase("auth");

var username = builder.AddParameter("username", secret: true);
var password = builder.AddParameter("password", secret: true);
var rabbitmq = builder.AddRabbitMQ("rabbitmq", username, password)
    .WithManagementPlugin();

builder.AddProject<Projects.Nunari_Auth_Api>("api")
    .WithReference(authdb).WaitFor(authdb)
    .WithReference(rabbitmq).WaitFor(rabbitmq);

builder.Build().Run();
