using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace doob.Reflectensions.Tests.Classes
{
    public class Building : CamouflageMode
    {
        private bool _mainDoorIsOpen;
        private int _floors;

        public Building() { }

        public Building(int floors) {
            _floors = floors;
        }

        public void OpenMainDoor() {
            _mainDoorIsOpen = true;
        }

        public async Task OpenMainDoorAsync(TimeSpan? delay = null) {
            await Task.Run(async () => {
                if (delay.HasValue) {
                    await Task.Delay(delay.Value);
                }
                OpenMainDoor();
            });
        }

        public void CloseDoor() {
            _mainDoorIsOpen = false;
        }

        public bool IsDoorOpen() {

            return _mainDoorIsOpen;
        }

        public int CountFloors() {
            return _floors;
        }

        public async Task<int> CountFloorsAsync(TimeSpan? delay = null) {
            return await Task.Run(async () => {
                if (delay.HasValue) {
                    await Task.Delay(delay.Value).ConfigureAwait(true);
                }
                return _floors;
            });
        }
    }
}
