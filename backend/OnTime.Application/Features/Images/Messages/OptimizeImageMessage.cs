using System;
using OnTime.Bus;

namespace OnTime.Application.Features.Images.Messages;

public record OptimizeImageMessage(Guid ImageId) : IBusMessage;
