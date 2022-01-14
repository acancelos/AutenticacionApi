## **Diseño de un servicio REST de autenticación único, reutilizable, genérico y extensible.**

Diseño de un componente genérico que contemple distintos métodos de autenticación y su consecuente generación y refresh de tokens.

Autenticación:
El tipo de autenticación se determina en la configuración del componente.

* Active Directory

  * Validación de usuario, password y obtención de grupos/roles por AD usando el protocolo LDAP.
  
  *	Validación de password por AD/LDAP, validar existencia de usuario a través de un servicio y obtención de roles a través de un campo en el registro de usuario.
  
  *	Validación de password por AD/LDAP, validar existencia de usuario y obtención de grupos o roles a través de un servicio.
  
*	Login Form / HTTP Authentication 

Ambos métodos son similares en términos de implementación. 
Para estos métodos se debe especificar configuración adicional respecto a la conexión de base de datos y esquema implementado con la que se hará la validación de usuario/contraseña y la obtención de roles. 

*	Custom authentication: validar usuario y password a través de un servicio personalizado, y obtención de roles y permisos ya sea a través de un campo en el registro de usuarios o a través de un servicio.

---
Todos los procesos de autenticación heredarán de la misma interface. En el método correspondiente al AD, el componente genérico tendrá la implementación. Con los otros métodos de autenticación la implementación se realizará de acuerdo a los requerimientos del proyecto.

El método de autenticación se determina según alguno de los proveedores de configuración.

![Diagrama de clases](https://github.com/acancelos/AutenticacionApi/blob/master/Class%20Mode2l.jpg?raw=true)

Para la generación del token se utilizará Jason Web Token (JWT) utilizando el esquema Bearer.

Paquetes de NuGet para generar los Token: **Microsoft.AspNetCore.Authentication.JwtBearer**

El servicio expondrá dos end-points:

*	Generar Token: Recibe un usuario y password y devuelve un token.

*	Refresh Token: Recibe un token. Si el token está vencido y se encuentra dentro del umbral de tolerancia de refresh devuelve un nuevo token. Si está fuera del umbral devuelve un 401 y deberá ser redirigido nuevamente al login.


![Diagrama de actividad](https://github.com/acancelos/AutenticacionApi/blob/master/Autenticacion.jpg?raw=true)
