using Microsoft.AspNetCore.SignalR;

namespace TrainingAndCertificationPlatform.Hubs;

// this hub handles the live "spots remaining" counter on a session page
public class EnrollmentHub : Hub
{
    // the client calls this when it opens a session details page
    // we put the connection in a group for that one session so it only
    // gets updates about the session the user is actually looking at
    public async Task JoinSession(int sessionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"session-{sessionId}");
    }

    // called when the user leaves the page so the connection stops getting updates
    public async Task LeaveSession(int sessionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"session-{sessionId}");
    }
}
