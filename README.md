# Desarrollo Web en Entorno Servidor - 02 - Desarrollo de servicios web en .NET

UD02. Desarrollo de servicios web en .NET. 2DAW. Curso 2026-2027

![imagen](https://github.com/joseluisgs/DesarrolloWebEntornosServidor-00-2023-2024/raw/master/images/servicios.png)

- [Desarrollo Web en Entorno Servidor - 02 - Desarrollo de servicios web en .NET](#desarrollo-web-en-entorno-servidor-02---desarrollo-de-servicios-web-en-net)
  - [Contenidos](#contenidos)
  - [Proyecto integrador](#proyecto-integrador)
  - [Contenido en YouTube](#contenido-en-youtube)
  - [Resultados de aprendizaje y criterios de evaluación](#resultados-de-aprendizaje-y-criterios-de-evaluación)
  - [Autor](#autor)
    - [Contacto](#contacto)
  - [Licencia de uso](#licencia-de-uso)


## Contenidos

0.  [Guía de supervivencia: comandos .NET CLI](00-comandos.md)
1.  [Servicios web](01-conceptos-servicios.md)
2.  [APIs REST](02-rest-api.md)
3.  [Minimal APIs](03-minimal-apis.md)
4.  [Controladores y MVC APIs](04-controladores-mvc.md)
5.  [Arquitectura y pipeline](05-arquitectura-pipeline.md)
6.  [Inyección de dependencias](06-inyeccion-dependencias.md)
7.  [Excepciones y patrón result](07-excepciones-patron-result.md)
8.  [DTOs, mapeadores, validaciones y consultas](08-dtos-mapeadores-validaciones.md)
9.  [Configuración y logging](09-configuracion-logging.md)
10. [Pruebas y despliegue básicos](10-pruebas-despliegue.md)
11. [Arquitecturas para servicios](11-clean-architecture.md)
12. [Entity Framework Core SQL](12-entity-framework-core.md)
13. [Entity Framework Core NoSQL con MongoDB](13-mongodb.md)
14. [Sistemas de caché: Redis y memcached](14-cache-redis.md)
15. [Transacciones, concurrencia e identificadores](15-transacciones-identificadores.md)
16. [Autenticación JWT y BCrypt](16-autenticacion.md)
17. [Autorización: roles, claims y políticas](17-autorizacion.md)
18. [Tiempo real con WebSockets y SignalR](18-websockets-signalr.md)
19. [Apis con GraphQL](19-graphql.md)
20. [Almacenamiento de ficheros](20-file-storage.md)
21. [Email services](21-email-services.md)
22. [Tareas programadas](22-tareas-programadas.md)
23. [Optimización de servicios web](23-optimizacion.md)
24. [Documentación mediante Swagger y OpenAPI](24-documentacion.md)
25. [Perfiles y configuración](25-perfiles.md)
26. [Organización e infraestructuras de Program.cs](26-organizacion-program.md)
27. [Logging y monitoreo](27-logging.md)
28. [Testing de servicios web](28-testing.md)
29. [Docker y despliegue](29-docker.md)
30. [CQRS: command query responsibility segregation](30-cqrs-mediator.md)
31. [API Gateway y microservicios](31-api-gateway.md)
32. [Resumen](32-resumen.md)

## Proyecto integrador
Los proyectos realizados en clase:
- [Proyecto integrador APIS](https://github.com/joseluisgs/TiendaDawApi-NetCore)
- [Proyecto integrador APIS CQRS](https://github.com/joseluisgs/TiendaDawApi-Cqrs-MediatR-NetCore)
  
## Contenido en YouTube
- [Resumen](https://youtu.be/FhjthcSROeo)
- [REST API](https://youtu.be/sMFroJJCKSI)
- [Entity Core framework y SQL](https://youtu.be/_xknwIXg6lI)
- [NoSQL y mongo con .NET/ASP Core](https://youtu.be/Ox7rGnrfx6Q)
- [WebSockets con .NET/ASP Core](https://youtu.be/zDSOj6atVsA)
- [GraphQL con .NET/ASP Core](https://youtu.be/9_slzfm5ods)
- [Caché avanzada con Redis en .NET/ASP Core](https://youtu.be/w95LVes-Bn4)
- [Seguridad: autenticación y autorización con .NET/ASP Core](https://youtu.be/LpP6EsaugXY)
- [CQRS y Mediator con .NET/ASP Core](https://youtu.be/ut8QgCSPGbo)
- [API Gateway y microservicios con .NET/ASP Core](https://youtu.be/o2WMCAlCnm4)
- [Lista de reproducción](https://www.youtube.com/playlist?list=PLLiuVpAc3Gv4)

## Resultados de aprendizaje y criterios de evaluación

- RA6: Desarrolla aplicaciones web de acceso a almacenes de datos, aplicando medidas para mantener la seguridad y la integridad de la información.

  - CCEE:

    - a) Se han analizado las tecnologías que permiten el acceso mediante programación a la información disponible en almacenes de datos.
    - b) Se han creado aplicaciones que establezcan conexiones con bases de datos.
    - c) Se ha recuperado información almacenada en bases de datos.
    - d) Se ha publicado en aplicaciones web la información recuperada.
    - e) Se han utilizado conjuntos de datos para almacenar la información.
    - f) Se han creado aplicaciones web que permitan la actualización y la eliminación de información disponible en una base de datos.
    - g) Se han probado y documentado las aplicaciones web.



- RA7: Desarrolla servicios web reutilizables y accesibles mediante protocolos web, verificando su funcionamiento.

  - CCEE:

    - a) Se han reconocido las características propias y el ámbito de aplicación de los servicios web.
    - b) Se han reconocido las ventajas de utilizar servicios web para proporcionar acceso a funcionalidades incorporadas a la lógica de negocio de una aplicación.
    - c) Se han identificado las tecnologías y los protocolos implicados en el consumo de servicios web.
    - d) Se han utilizado los estándares y arquitecturas más difundidos e implicados en el desarrollo de servicios web.
    - e) Se ha programado un servicio web.
    - f) Se ha verificado el funcionamiento del servicio web.
    - g) Se ha consumido el servicio web.
    - h) Se ha documentado un servicio web.



- RA9: Desarrolla aplicaciones web híbridas seleccionando y utilizando tecnologías, frameworks servidor y repositorios heterogéneos de información.

  - CCEE:

    - a) Se han reconocido las ventajas que proporciona la reutilización de código y el aprovechamiento de información ya existente.
    - b) Se han identificado tecnologías y frameworks aplicables en la creación de aplicaciones web híbridas.
    - e) Se han utilizado librerías de código y frameworks para incorporar funcionalidades específicas a una aplicación web.
    - g) Se han analizado y utilizado librerías de código relacionadas con Big Data e inteligencia de negocios, para incorporar análisis e inteligencia de datos proveniente de repositorios.
    - h) Se han probado, depurado y documentado las aplicaciones generadas.


## Autor

Codificado con :sparkling_heart: por [José Luis González Sánchez](https://joseluisgs.dev)

[![Twitter](https://img.shields.io/twitter/follow/JoseLuisGS_?style=social)](https://x.com/JoseLuisGSDev)
[![GitHub](https://img.shields.io/github/followers/joseluisgs?style=social)](https://github.com/joseluisgs)
[![GitHub](https://img.shields.io/github/stars/joseluisgs?style=social)](https://github.com/joseluisgs)

### Contacto

<p>
  Cualquier cosa que necesites házmelo saber por si puedo ayudarte 💬.
</p>
<p>
    <a href="https://joseluisgs.dev/" target="_blank">
        <img loading="lazy" src="https://github.com/joseluisgs/joseluisgs/raw/master/images/social-icons/favicon.png" height="32">
    </a>&nbsp;
    <a href="https://github.com/joseluisgs" target="_blank">
        <img loading="lazy" src="https://github.com/joseluisgs/joseluisgs/raw/master/images/social-icons/github.svg" height="32">
    </a>&nbsp;
    <a href="https://www.linkedin.com/in/JoseLuisGSDev" target="_blank">
        <img loading="lazy" src="https://github.com/joseluisgs/joseluisgs/raw/master/images/social-icons/linkedin.png" height="32">
    </a>&nbsp;
    <a href="https://www.youtube.com/@joseluisgs" target="_blank">
        <img loading="lazy" src="https://github.com/joseluisgs/joseluisgs/raw/master/images/social-icons/youtube.png" height="32">
    </a>&nbsp;
    <a href="https://x.com/JoseLuisGSDev" target="_blank">
        <img loading="lazy" src="https://github.com/joseluisgs/joseluisgs/raw/master/images/social-icons/twitter.png" height="32">
    </a>&nbsp;
    <a href="https://www.instagram.com/joseluisgs.dev/" target="_blank">
        <img loading="lazy" src="https://github.com/joseluisgs/joseluisgs/raw/master/images/social-icons/instagram.png" height="32">
    </a>
</p>

## Licencia de uso

Este repositorio y todo su contenido está licenciado bajo licencia **Creative Commons**, si desea saber más, vea
la [LICENSE](https://joseluisgs.dev/docs/license/). Por favor si compartes, usas o modificas este proyecto cita a su
autor, y usa las mismas condiciones para su uso docente, formativo o educativo y no comercial.

<a rel="license" href="http://creativecommons.org/licenses/by-nc-sa/4.0/"><img alt="Licencia de Creative Commons" style="border-width:0" src="https://i.creativecommons.org/l/by-nc-sa/4.0/88x31.png" /></a><br /><span xmlns:dct="http://purl.org/dc/terms/" property="dct:title">
JoseLuisGS</span> by <a xmlns:cc="http://creativecommons.org/ns#" href="https://joseluisgs.dev/" property="cc:attributionName" rel="cc:attributionURL">
José Luis González Sánchez</a> is licensed under
<a rel="license" href="http://creativecommons.org/licenses/by-nc-sa/4.0/">Creative Commons
Reconocimiento-NoComercial-CompartirIgual 4.0 Internacional License</a>.<br />Creado a partir de la obra
en <a xmlns:dct="http://purl.org/dc/terms/" href="https://github.com/joseluisgs" rel="dct:source">https://github.com/joseluisgs</a>.
