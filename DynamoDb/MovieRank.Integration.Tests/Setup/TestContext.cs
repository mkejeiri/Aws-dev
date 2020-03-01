using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace MovieRank.Integration.Tests.Setup
{
    public class TestContext :
        //Why might we need IAsyncLifetime when we could call or initialize any method from inside out constructor?
        //here we pull down, run & start a local instance of DynamoDB Inside a docker container, we need to call InitializeAsync
        //Calling asynchronous methods from a constructor is tough work when trying to avoid deadlocks
        IAsyncLifetime
    {

        //1- The purpose here is to spin up and run an in-memory local instance of dynamoDB with a docker container
        //2- we want also to set up, in-memory to TestServer and HttpClient This will allow us to spin up an in-memory instance of the app and make requests to it. 

        //TestServer settings
        private TestServer _server;
        public HttpClient Client { get; private set; }

        //dynamoDB local Url 
        private const string Uri = "http://localhost:8000";

        //Docker settings
        private readonly DockerClient _dockerClient;
        private string _containerId;
        private const string ContainerImageUri = "amazon/dynamodb-local";
        private const string Port = "8000";
        public TestContext()
        {
            SetupClient();
            _dockerClient = new DockerClientConfiguration(new Uri("npipe://./pipe/docker_engine")).CreateClient();
        }

        //this method will be run to directly after the classes constructor!
        public async Task InitializeAsync()
        {
            await PullImage();
            await StartContainer();
            await new TestDataSetup().CreateTable();
        }


        //Kill the container
        public async Task DisposeAsync()
        {
            if (_containerId !=null)
            {
                await _dockerClient.Containers.KillContainerAsync(_containerId, new ContainerKillParameters());
            }
        }

        private async Task StartContainer()
        {
            var createContainerResponse = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters()
            {
                Image = ContainerImageUri,

                //We want to expose the Port=8000 to allow us to interact with docker container
                ExposedPorts = new Dictionary<string, EmptyStruct>()
                {
                    { Port, default(EmptyStruct) }
                },
                HostConfig = new HostConfig()
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>()
                    {
                        {Port, new List<PortBinding>() {new PortBinding() {HostPort = Port } }}
                    },

                    //We need to ensure that all of our ports are published
                    PublishAllPorts = true
                }
            });

            _containerId = createContainerResponse.ID;

            //Start Container

            await _dockerClient.Containers.StartContainerAsync(_containerId,

                //we don't need to add any containers start parameters
                null);
        }
        private async Task PullImage()
        {
            await _dockerClient.Images.CreateImageAsync(
                new ImagesCreateParameters
                {
                    FromImage = ContainerImageUri,
                    Tag = "latest"
                },
                new AuthConfig(), //we might need to add a username/password to access at Dhaka image,
                //however, we were able to download the dynamoDb local docker image
                //anonymously so we can leave this empty

                new Progress<JSONMessage>() //This gives us the ability to check on the progress of pulling down the doctor image
            );
        }

        private void SetupClient()
        {

            _server = new TestServer(new WebHostBuilder()
                .UseEnvironment("Development")
                .UseStartup<Startup>());


            _server.BaseAddress = new Uri(Uri);
            Client = _server.CreateClient();
        }
    }
}