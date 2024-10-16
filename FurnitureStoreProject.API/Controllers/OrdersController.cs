﻿using AutoMapper;
using FurnitureStore.API.DTOs.OrderDTOs;
using FurnitureStore.API.DTOs.ProductCategoryDTOs;
using FurnitureStore.API.Entities;
using FurnitureStore.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FurnitureStore.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersRepository _repository;
        private readonly IUsersRepository _usersRepository;
        private readonly IMapper _mapper;
        public OrdersController(IOrdersRepository repository, IUsersRepository usersRepository, IMapper mapper)
        {
            _repository = repository;
            _usersRepository = usersRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public ActionResult<List<Order>> GetAll()
        {
            string role = User.Claims.FirstOrDefault(c => c.Type == "role")?.Value ?? "Client";

            if (role != "Admin")
            {
                return Unauthorized("Not authorized to view users.");
            }

            List<Order> orders = _repository.GetAllOrders().ToList();
            if (orders.Count == 0)
                return NotFound("The order list is empty");

            //var orderDto = _mapper.Map<List<OrderDto>>(orders);
            return Ok(orders);
        }


        [HttpGet("{idOrder}", Name = "GetOrder")]
        [Authorize]
        public IActionResult GetOrder(int idOrder)
        {
            string role = User.Claims.FirstOrDefault(c => c.Type == "role")?.Value ?? "Client";

            if (role != "Admin")
            {
                return Unauthorized("Not authorized to view orders.");
            }

            if (!_repository.OrderExists(idOrder))
                return NotFound();

            Entities.Order? order = _repository.GetOrderById(idOrder);
            var orderDto = _mapper.Map<OrderDto>(order);

            return Ok(orderDto);
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public IActionResult GetOrdersByUserId(int userId)
        {
            string role = User.Claims.FirstOrDefault(c => c.Type == "role")?.Value ?? "Client";

            if (role != "Admin")
            {
                return Unauthorized("Not authorized to view orders.");
            }

            //if (!_repository.OrderExists(idOrder))
            //    return NotFound();

            List<Order> orders = _repository.GetOrdersByUserId(userId).ToList();

            if (orders.Count == 0) 
                return NotFound("Order list is empty");

            var orderDto = _mapper.Map<OrderDto>(orders);

            return Ok(orderDto);
        }

        [HttpGet("user")]
        [Authorize]
        public IActionResult GetOrdersByCurrentUser()
        {
            int userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? "0");

            var user = _usersRepository.GetUser(userId); // Obtener el usuario por el ID del token

            if (user is null)
            {
                return NotFound("The user does not exist");
            }

            //if (!_repository.OrderExists(idOrder))
            //    return NotFound();

            List<Order> orders = _repository.GetOrdersByUserId(userId).ToList();

            if (orders.Count == 0)
                return NotFound("Order list is empty");

            //var orderDto = _mapper.Map<OrderDto>(orders);

            return Ok(orders);
        }

        [HttpPost]
        public IActionResult CreateOrder([FromBody] OrderToCreateDto orderToCreate)
        {
            if (orderToCreate == null)
                return BadRequest();

            int userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value);

            var newOrder = _mapper.Map<Order>(orderToCreate);

            newOrder.UserId = userId;

            newOrder.OrderDate = DateTime.UtcNow; // Asigna la fecha actual

            _repository.AddOrder(newOrder);

            return Created("Created", orderToCreate);
        }

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateOrder(int id)
        {
            string role = User.Claims.FirstOrDefault(c => c.Type == "role")?.Value ?? "Client";

            if (role != "Admin")
            {
                return Unauthorized("Not authorized to update order.");
            }

            Order? order = _repository.GetOrderById(id);
            if (order == null)
                return NotFound();

            order.IsCompleted = !order.IsCompleted;

            var orderDetails = order.OrderDetails.ToList();

            _repository.Update(order, orderDetails);

            _repository.SaveChanges();

            return NoContent();

        }


        [HttpDelete("{idOrder}")]
        [Authorize]
        public IActionResult DeleteOrder(int idOrder)
        {
            string role = User.Claims.FirstOrDefault(c => c.Type == "role")?.Value ?? "Client";

            if (role != "Admin")
            {
                return Unauthorized("Not authorized to delete orders.");
            }

            if (!_repository.OrderExists(idOrder))
                return NotFound();

            var orderToDelete = _repository.DeleteOrderAndDetails(idOrder);

            if(orderToDelete == false)
                return NotFound();

            _repository.SaveChanges();

            return NoContent();
        }

    }
}
