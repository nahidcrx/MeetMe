using AutoMapper;
using MeetMe.DTOs;
using MeetMe.Entities;
using MeetMe.Extentions;
using MeetMe.Helpers;
using MeetMe.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetMe.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public MessagesController(IUserRepository userRepository, 
            IMessageRepository messageRepository, IMapper mapper) 
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto) 
        {
            var username = User.GetUserName();
            if (username == createMessageDto.RecipientUsername.ToLower())
                return BadRequest("You cannot send messages to yourself");
            var sender = await _userRepository.GetUserByUserNameAsync(username);
            var recipient = await _userRepository.GetUserByUserNameAsync(createMessageDto.RecipientUsername);

            if (recipient == null) return NotFound();

            var messsage = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content,

            };

            _messageRepository.AddMessage(messsage);

            if (await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(messsage));

            return BadRequest("Failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams) 
        {
            messageParams.Username = User.GetUserName();
            var messages = await _messageRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize,
                messages.TotalCount, messages.TotalPages);

            return messages;
        }

        [HttpGet("thread/{username}")]

        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username) 
        {
            var currentUsername = User.GetUserName();

            return Ok(await _messageRepository.GetMessagesThread(currentUsername, username));
        }

        [HttpDelete("{id}")]

        public async Task<ActionResult> DeleteMessage(int id) 
        {
            var currentUsername = User.GetUserName();
            var message = await _messageRepository.GetMessage(id);

            if (message.Sender.UserName != currentUsername && message.Recipient.UserName != currentUsername)
                return Unauthorized();
            if (message.Sender.UserName == currentUsername) message.SenderDeleted = true;
            if (message.Recipient.UserName == currentUsername) message.RecipientDeleted = true;

            if (message.SenderDeleted && message.SenderDeleted) 
                _messageRepository.DeleteMessage(message);

            if (await _messageRepository.SaveAllAsync()) return Ok();

            return BadRequest("Problems occur while deleting message. Please try again !");


        }
    }
}
