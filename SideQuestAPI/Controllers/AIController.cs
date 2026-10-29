
using Microsoft.AspNetCore.Mvc;
using SideQuestAPI.Services;

namespace SideQuestAPI.Controllers
{
    [ApiController]
    [Route("api/ai")]
    public class AIController : ControllerBase
    {
        private readonly WeeklyQuestService _weeklyQuestService;

        public AIController(WeeklyQuestService weeklyQuestService)
        {
            _weeklyQuestService = weeklyQuestService;
        }

        [HttpGet("quests")]
        public async Task<IActionResult> GetQuests()
        {
            try
            {
                var quests = await _weeklyQuestService
                    .GetOrGenerateWeeklyQuestsAsync();

                return Ok(new
                {
                    quests = quests.Select(q => new
                    {
                        id = q.QuestId,
                        title = q.Title,
                        description = q.Description,
                        xp_reward = q.XpReward,
                        photo_criteria = q.PhotoCriteria
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Failed to retrieve weekly quests.",
                    error = ex.Message
                });
            }
        }

        [HttpGet("weekly-status")]
        public IActionResult GetWeeklyStatus()
        {
            return Ok(new
            {
                nextRerollUtc = WeeklyQuestService.GetNextRerollUtc()
            });
        }

    }
}
