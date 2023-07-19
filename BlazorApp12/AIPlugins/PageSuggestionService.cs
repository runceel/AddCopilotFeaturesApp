using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning;
using Microsoft.SemanticKernel.SkillDefinition;
using System.Text.Json;

namespace BlazorApp12.AIPlugins;

public class PageSuggestionService
{
    private IKernel _kernel;

    public PageSuggestionService(IKernel kernel, IPageInfoProvider pageInfoProvider)
    {
        ImportPagePlugin(kernel, pageInfoProvider.GetPages());
        _kernel = kernel;
    }

    public static void ImportPagePlugin(IKernel kernel, IEnumerable<PageInfo> pages)
    {
        foreach (var page in pages)
        {
            var json = JsonSerializer.Serialize(page);
            kernel.RegisterCustomFunction(
                SKFunction.FromNativeFunction(
                    () => json,
                    nameof(PageInfoProvider),
                    page.Name,
                    page.Description));
        }
    }

    public async Task<PageInfo?> SuggestPageAsync(string goal)
    {
        var planner = new ActionPlanner(_kernel);
        var plan = await planner.CreatePlanAsync(goal);
        if (!plan.HasNextStep)
        {
            return null;
        }

        var planResult = await plan.InvokeAsync();
        return JsonSerializer.Deserialize<PageInfo>(planResult.Result);
    }
}
