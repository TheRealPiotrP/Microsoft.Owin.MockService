[![Build status](https://ci.appveyor.com/api/projects/status/whk9fannq0xe7gjx?svg=true)](https://ci.appveyor.com/project/piotrpMSFT/microsoft-owin-mockservice)

# Microsoft.Owin.MockService
The MockService enables developers to setup, configure, and dispose of a functioning Web server in the context of a Unit Test using a simple fluent interface.

```csharp
[Fact]
public void MockService_can_be_configured_and_invoked_in_a_unit_test()
{
  using (var mockService = new MockService()
      .OnRequest(r => r.Path.ToString() == "/SayHello")
      .RespondWith(r => r.Write("Hello, World!")))
  {
      var client = new HttpClient()
      {
           BaseAddress = new Uri(mockService.GetBaseAddress());
      }
  
      client.GetStringAsync("/SayHello").Result
          .Should().Be("Hello, World!");
  }
}
```
