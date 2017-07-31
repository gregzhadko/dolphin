using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using Telerik.Windows.Data;
using Timeline.Model;

namespace Timeline.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private RadObservableCollection<BaseBox> _newBoxList;

        /// <summary>
        ///     Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            BoxesInTimeline = new ObservableCollection<BaseBox>();
            NewBoxList = new RadObservableCollection<BaseBox> {NewNSRBox(), NewPVCBox(), NewPauseBox()};
            BoxesInTimeline.Add(NewNSRBox());
            NewBoxList.CollectionChanged += NewBoxListOnCollectionChanged;
        }

        /// <summary>
        ///     Collect all the boxes which were added to the timeline
        /// </summary>
        public ObservableCollection<BaseBox> BoxesInTimeline { get; set; }

        /// <summary>
        ///     Return total duration of boxes in the timeline
        /// </summary>
        public int TotalDuration
        {
            get { return BoxesInTimeline.Sum(b => b.Duration); }
        }

        /// <summary>
        ///     List of the boxes which can be added. For now it contains two boxes: Pause Box and NSR Box.
        ///     If any of the box is removed a new one will be created
        /// </summary>
        public RadObservableCollection<BaseBox> NewBoxList
        {
            get => _newBoxList;
            set => Set(ref _newBoxList, value);
        }

        /// <summary>
        ///     Command which occurs when user click export command
        /// </summary>
        public RelayCommand ExportCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    //Get the file
                    var fileDialog = new SaveFileDialog
                    {
                        Filter = "Text Files | *.txt",
                        DefaultExt = "txt"
                    };
                    if (fileDialog.ShowDialog() != true || !fileDialog.CheckPathExists) return;
                    WriteData(fileDialog.FileName);
                });
            }
        }

        private void WriteData(string path)
        {
            using (var stream = new StreamWriter(path))
            {
                //write 256 line at beginning of document
                stream.WriteLine(256);

                //write number of boxes following 256 line (totalEntries = TotalDuration of timeline * 256)
                var totalEntries = TotalDuration * 256;
                stream.WriteLine(totalEntries);

                var boxes = RemoveDoublePauseBoxes(BoxesInTimeline.ToList());

                foreach (var box in boxes.OfType<Box>())
                {
                    box.GeneratePoints();
                    //find peak value
                    box.CalculateMaxValues();
                }

                //Get points for Pause boxes. 
                //Number of points depends on max point location of neighbor boxes 
                for (var i = 0; i < boxes.Count; i++)
                {
                    var pauseBox = boxes[i] as PauseBox;
                    pauseBox?.GeneratePoints(i > 0 ? (Box) boxes[i - 1] : null, i + 1 < boxes.Count ? (Box) boxes[i + 1] : null);
                }

                stream.WriteLine(boxes.Count);
                //Finally write the points to the file
                foreach (var box in boxes)
                {
                    foreach (var point in box.Points)
                    {
                        stream.WriteLine(point);
                    }
                }
            }
        }

        /// <summary>
        ///     Union pause boxes if they are located one after another in the list
        /// </summary>
        /// <param name="boxes">Source list</param>
        /// <returns>Result list</returns>
        private static List<BaseBox> RemoveDoublePauseBoxes(IReadOnlyList<BaseBox> boxes)
        {
            var result = new List<BaseBox>();
            for (var i = 0; i < boxes.Count; i++)
            {
                var box = boxes[i];
                result.Add(box);
                if (box is PauseBox)
                {
                    while (i + 1 < boxes.Count && boxes[i + 1] is PauseBox)
                    {
                        box.Duration += boxes[++i].Duration;
                    }
                }
            }

            return result;
        }

        private void NewBoxListOnCollectionChanged(object sender,
            NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            Task.Run(() => Thread.Sleep(150))
                .ContinueWith(r => Application.Current.Dispatcher.Invoke(() =>
                {
                    //Check if in the list only one item
                    //Add a new box
                    if (NewBoxList.Count == 2)
                    {
                        if (!(NewBoxList[NewBoxList.Count - 1] is PauseBox))
                        {
                            NewBoxList.Add(NewPauseBox());
                        }
                        else if (IsBoxWithTitleExists("NSR"))
                        {
                            NewBoxList.Insert(0, NewNSRBox());
                        }
                        else if (IsBoxWithTitleExists("PVC"))
                        {
                            NewBoxList.Insert(1, NewPVCBox());
                        }
                    }
                }));

            RaisePropertyChanged(() => TotalDuration);
        }

        private bool IsBoxWithTitleExists(string title)
        {
            return NewBoxList.All(b => b.Title != title);
        }


        private static Box NewNSRBox()
        {
            return new Box("NSR", "NSR") {Duration = 2, Bpm = 20};
        }

        private static Box NewPVCBox()
        {
            return new Box("PVC", "NSR") {Duration = 2, Bpm = 20};
        }


        private static PauseBox NewPauseBox()
        {
            return new PauseBox {Duration = 2};
        }
    }
}