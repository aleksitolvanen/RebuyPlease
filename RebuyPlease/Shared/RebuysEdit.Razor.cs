using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using EventAggregator.Blazor;
using Microsoft.AspNetCore.Components;
using RebuyPlease.Data;

namespace RebuyPlease.Shared
{
    public partial class RebuysEdit : ComponentBase, IHandle<Rebuys>
    {
        [Inject]
        private IEventAggregator _eventAggregator { get; set; }

        [Inject]
        private RebuyService rebuyService { get; set; }

        private Rebuys rebuys;

        private AddPlayerModel addPlayerModel { get; set; } = new AddPlayerModel();

        private AddTournamentModel addTournamentModel { get; set; } = new AddTournamentModel();



        protected override void OnInitialized()
        {
            rebuys = rebuyService.Rebuys;
            _eventAggregator.Subscribe(this);
        }

        private async Task AddRebuy(string player, string tournament, int count)
        {
            rebuyService.Rebuys.AddRebuy(player, tournament, count);
            var rebuys = rebuyService.Rebuys;
            await _eventAggregator.PublishAsync(rebuys);
        }

        public Task HandleAsync(Rebuys rebuysClass)
        {
            rebuys = rebuysClass;
            return Task.CompletedTask;
        }

        private void HandlePlayerAdd()
        {
            rebuys.AddPlayer(addPlayerModel.Name);
        }

        private void HandleTournamentAdd()
        {
            rebuys.AddTournament(addTournamentModel.Name);
        }
    }

    public class AddPlayerModel
    {
        [Required]
        [StringLength(15, ErrorMessage = "Name is too long.")]
        public string Name { get; set; }
    }

    public class AddTournamentModel
    {
        [Required]
        [StringLength(25, ErrorMessage = "Name is too long.")]
        public string Name { get; set; }
    }
}
