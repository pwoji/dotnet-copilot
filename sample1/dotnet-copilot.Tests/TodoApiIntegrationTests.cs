using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace dotnet_copilot.Tests;

public class TodoApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TodoApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateTodo_ShouldReturnCreated()
    {
        var todo = new { Title = "Test Todo", IsCompleted = false };
        var response = await _client.PostAsJsonAsync("/api/todos", todo);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<TodoDto>();
        created.Should().NotBeNull();
        created!.Title.Should().Be(todo.Title);
        created.IsCompleted.Should().BeFalse();
    }

    [Fact]
    public async Task GetTodo_ShouldReturnTodo()
    {
        var todo = new { Title = "Read Test", IsCompleted = false };
        var createResp = await _client.PostAsJsonAsync("/api/todos", todo);
        var created = await createResp.Content.ReadFromJsonAsync<TodoDto>();
        var response = await _client.GetAsync($"/api/todos/{created!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var fetched = await response.Content.ReadFromJsonAsync<TodoDto>();
        fetched.Should().NotBeNull();
        fetched!.Title.Should().Be(todo.Title);
    }

    [Fact]
    public async Task GetTodos_ShouldReturnList()
    {
        var response = await _client.GetAsync("/api/todos");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await response.Content.ReadFromJsonAsync<List<TodoDto>>();
        list.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateTodo_ShouldReturnNoContent()
    {
        var todo = new { Title = "Update Test", IsCompleted = false };
        var createResp = await _client.PostAsJsonAsync("/api/todos", todo);
        var created = await createResp.Content.ReadFromJsonAsync<TodoDto>();
        var update = new { Title = "Updated", IsCompleted = true };
        var response = await _client.PutAsJsonAsync($"/api/todos/{created!.Id}", update);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var getResp = await _client.GetAsync($"/api/todos/{created.Id}");
        var updated = await getResp.Content.ReadFromJsonAsync<TodoDto>();
        updated!.Title.Should().Be("Updated");
        updated.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteTodo_ShouldReturnNoContent()
    {
        var todo = new { Title = "Delete Test", IsCompleted = false };
        var createResp = await _client.PostAsJsonAsync("/api/todos", todo);
        var created = await createResp.Content.ReadFromJsonAsync<TodoDto>();
        var response = await _client.DeleteAsync($"/api/todos/{created!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var getResp = await _client.GetAsync($"/api/todos/{created.Id}");
        getResp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetTodo_NotFound_ShouldReturn404()
    {
        var response = await _client.GetAsync("/api/todos/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateTodo_InvalidInput_ShouldReturn400()
    {
        var todo = new { Title = "", IsCompleted = false };
        var response = await _client.PostAsJsonAsync("/api/todos", todo);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

public record TodoDto(int Id, string Title, bool IsCompleted);
