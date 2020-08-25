using Prism.Events;
using Prism.Mvvm;
using PrismApp.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace RightModule.ViewModels
{
    public class MessageListViewModel : BindableBase
    {
        IEventAggregator _ea;

        // Messages
        private ObservableCollection<string> messages;
        public ObservableCollection<string> Messages
        {
            get => messages;
            set => SetProperty(ref messages, value);
        }

        public MessageListViewModel(IEventAggregator ea)
        {
            _ea = ea;
            Messages = new ObservableCollection<string>();
            _ea.GetEvent<MessageSentEvent>().Subscribe(MessageReceived, ThreadOption.PublisherThread, false,
                filter => filter.Contains("Prism")); // Prism이 포함된 문자만 받아서 넣음
        }

        private void MessageReceived(string message)
        {
            Messages.Add(message);
        }
    }
}
