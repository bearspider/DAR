using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace HEAP
{
    public class TreeViewModel : INotifyPropertyChanged
    {        
        public TreeViewModel(string name)
        {
            Name = name;
            _children = new ObservableCollection<TreeViewModel>();
        }
        #region Properties
        public string Name { get; private set; }
        private ObservableCollection<TreeViewModel> _children;
        public bool IsInitiallySelected { get; private set; }
        public string Type { get; set; }
        public String Id { get; set; }
        bool? _isChecked = false;
        TreeViewModel _parent;
        #endregion
        #region IsChecked
        public bool? IsChecked
        {
            get { return _isChecked; }
            set { SetIsChecked(value, true, true); }
        }
        void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == _isChecked)
            {
                return;
            }
            if(_isChecked == false)
            {
                NotifyTriggerAdded(Id);
            }
            else
            {
                NotifyTriggerRemoved(Id);
            }
            _isChecked = value;

            if (updateChildren && _isChecked.HasValue)
            {
                foreach(TreeViewModel tvm in _children)
                {
                    tvm.SetIsChecked(_isChecked, true, false);
                }
            }
            if (updateParent && _parent != null) _parent.VerifyCheckedState();
            NotifyPropertyChanged("IsChecked");
            
        }
        public void Enable()
        {
            SetIsChecked(true, true, true);
        }
        public void Disable()
        {
            SetIsChecked(false, true, true);
        }
        public void VerifyCheckedState()
        {
            bool? state = null;

            for (int i = 0; i < Children.Count; ++i)
            {
                bool? current = Children[i].IsChecked;
                if (i == 0)
                {
                    state = current;
                }
                else if (state != current)
                {
                    state = null;
                    break;
                }
            }

            SetIsChecked(state, false, true);
        }
        #endregion
        public void Initialize()
        {
            foreach (TreeViewModel child in _children)
            {
                child._parent = this;
                child.Initialize();
            }            
        }
        public void RemoveChild(TreeViewModel removeview)
        {
            _children.Remove(removeview);
            NotifyBranchChanged("RemoveChild");
        }
        private Boolean ContainsGuid(string guid)
        {
            Boolean rval = false;
            foreach(TreeViewModel tvm in _children)
            {
                if(tvm.Id == guid)
                {
                    rval = true;
                }
            }
            return rval;
        }
        private TreeViewModel GetBranch(string guid)
        {
            foreach(TreeViewModel tvm in _children)
            {
                if(tvm.Id == guid)
                {
                    return tvm;
                }
            }
            return null;
        }
        public void RemoveBranch(string guid)
        {
            if(ContainsGuid(guid))
            {
                RemoveChild(GetBranch(guid));
            }
            else
            {
                foreach(TreeViewModel tvm in _children)
                {
                    tvm.RemoveBranch(guid);
                }
            }
        }
        public void AddBranch(string guid)
        {

        }
        public void AddChild(TreeViewModel addview)
        {
            _children.Add(addview);
            NotifyBranchChanged("AddChild");
        }
        public ObservableCollection<TreeViewModel> Children
        {
            get { return _children; }
            private set { }
        }
        public static ObservableCollection<TreeViewModel> SetTree(string topLevelName)
        {
            ObservableCollection<TreeViewModel> treeView = new ObservableCollection<TreeViewModel>();
            TreeViewModel tv = new TreeViewModel(topLevelName);
            treeView.Add(tv);
            tv.Initialize();
            return treeView;
        }
        #region INotifyPropertyChanged Members
        void NotifyTriggerRemoved(string info)
        {
            if(TriggerRemoved != null)
            {
                TriggerRemoved(this, new PropertyChangedEventArgs(info));
            }
        }
        void NotifyTriggerAdded(string info)
        {
            if (TriggerAdded != null)
            {
                TriggerAdded(this, new PropertyChangedEventArgs(info));
            }
        }
        void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        void NotifyBranchChanged(string info)
        {
            if(BranchChanged != null)
            {
                BranchChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangedEventHandler TriggerAdded;
        public event PropertyChangedEventHandler TriggerRemoved;
        public event PropertyChangedEventHandler BranchChanged;

        #endregion
    }
}
