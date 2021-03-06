﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    internal class ColumnHeaderActionCommand : DataGridCommand
    {
        private const string SortKey = "Sort";
        private const string FilterKey = "Filter";
        private const string GroupKey = "Group";

        public override bool CanExecute(object parameter)
        {
            var context = parameter as ColumnHeaderActionContext;
            if(context == null)
            {
                return false;
            }

            var key = context.Key;
            bool isActionAllowed = false;

            if(key.Equals(ColumnHeaderActionCommand.FilterKey))
            {
               isActionAllowed = context.ColumnHeader.Column.CanUserFilter && context.ColumnHeader.Owner.UserFilterMode != DataGridUserFilterMode.Disabled;
            }
            else if(key.Equals(ColumnHeaderActionCommand.SortKey))
            {
                isActionAllowed = context.ColumnHeader.Column.CanUserSort && context.ColumnHeader.Owner.UserSortMode != DataGridUserSortMode.None;
            }
            else if(key.Equals(ColumnHeaderActionCommand.GroupKey))
            {
                isActionAllowed = context.ColumnHeader.Column.CanUserGroup && context.ColumnHeader.Owner.UserGroupMode != DataGridUserGroupMode.Disabled;
            }

            return isActionAllowed;
        }

        private Dictionary<DataGridColumn, GroupDescriptorBase> appliedGroupDescriptors = new Dictionary<DataGridColumn, GroupDescriptorBase>();
        public override void Execute(object parameter)
        {
            var context = parameter as ColumnHeaderActionContext;

            if (context == null)
            {
                return;
            }

            this.Owner.ContentFlyout.Hide(DataGridFlyoutId.ColumnHeader);

            if (context.Key.Equals(ColumnHeaderActionCommand.FilterKey))
            {
                this.Owner.ExecuteFilter(context.ColumnHeader);
            }
            else if (context.Key.Equals(ColumnHeaderActionCommand.SortKey))
            {
                context.ColumnHeader.Column.ToggleSort(this.Owner.UserSortMode == DataGridUserSortMode.Multiple);
            }
            else if (context.Key.Equals(ColumnHeaderActionCommand.GroupKey))
            {
                var column = context.ColumnHeader.Column;

                if (column.CanGroupBy)
                {
                    var descriptor = column.GetGroupDescriptor();

                    this.Owner.GroupDescriptors.Add(descriptor);
                }
                else
                {
                    var typedColumn = column as DataGridTypedColumn;
                    if (typedColumn != null)
                    {
                        var descriptor = this.Owner.GroupDescriptors.OfType<PropertyGroupDescriptor>().Where(d => d.PropertyName == typedColumn.PropertyName).FirstOrDefault();
                        if (descriptor != null)
                        {
                            this.Owner.GroupDescriptors.Remove(descriptor);
                        }
                    }            
                }
    
             }
        }    
       
    }
}
