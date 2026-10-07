namespace ProductosExcepciones.Errors;

public class DomainException(string message) : Exception(message);

public class NotFoundException(string message) : DomainException(message);
public class ValidationException(string message) : DomainException(message);
public class ConflictException(string message) : DomainException(message);
