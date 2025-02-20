using BotSharp.Abstraction.Agents.Models;

namespace BotSharp.Core.Agents.Services;

public partial class AgentService
{
    public async Task<Agent> LoadAgent(string id)
    {
        var hooks = _services.GetServices<IAgentHook>();

        // Before agent is loaded.
        foreach (var hook in hooks)
        {
            hook.OnAgentLoading(ref id);
        }

        var agent = await GetAgent(id);

        // After agent is loaded
        foreach (var hook in hooks)
        {
            hook.SetAget(agent);

            if (!string.IsNullOrEmpty(agent.Instruction))
            {
                hook.OnInstructionLoaded(agent.Instruction, new Dictionary<string, object>());
            }

            if (!string.IsNullOrEmpty(agent.Functions))
            {
                var functions = agent.Functions;
                hook.OnFunctionsLoaded(ref functions);
            }

            if (!string.IsNullOrEmpty(agent.Samples))
            {
                var samples = agent.Samples;
                hook.OnSamplesLoaded(ref samples);
            }

            hook.OnAgentLoaded(agent);
        }

        _logger.LogInformation($"Loaded agent {agent}.");

        return agent;
    }
}
