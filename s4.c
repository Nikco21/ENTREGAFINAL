#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <pthread.h>
#include <mysql/mysql.h>


int contador;
pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;
int i;
int sockets[100];
int autenticados[100];

typedef struct {
	char nombre[20];
	int socket;
} Conectado;

typedef struct {
	Conectado conectados[100];
	int numConectados;
} ListaConectados;

ListaConectados *listaConectados;

int PonerConectado(ListaConectados *lista, char nombre[20], int socket) {
	pthread_mutex_lock(&mutex);
	if (lista->numConectados >= 100) {
		pthread_mutex_unlock(&mutex);
		return -1;
	} else {
		strcpy(lista->conectados[lista->numConectados].nombre, nombre);
		lista->conectados[lista->numConectados].socket = socket;
		lista->numConectados++;
		printf("Jugadores conectados (%d): ", lista->numConectados);
		for (int i = 0; i < lista->numConectados; i++) {
			printf("%s ", lista->conectados[i].nombre);
		}
		printf("\n");
		pthread_mutex_unlock(&mutex);
		return 0;
	}
}

int EliminarConectado(ListaConectados *lista, char nombre[20]) {
	pthread_mutex_lock(&mutex);
	int posicion = -1;
	for (int j = 0; j < lista->numConectados; j++) {
		if (strcmp(lista->conectados[j].nombre, nombre) == 0) {
			posicion = j;
			break;
		}
	}
	if (posicion == -1) {
		pthread_mutex_unlock(&mutex);
		return -1;  // No encontrado
	}
	
	// Desplazamos el resto
	for (int j = posicion; j < lista->numConectados - 1; j++) {
		lista->conectados[j] = lista->conectados[j + 1];
	}
	
	lista->numConectados--;
	pthread_mutex_unlock(&mutex);
	return 0;
}

void DameConec(ListaConectados *lista, char nombres[512]) {
	pthread_mutex_lock(&mutex);
	if (lista->numConectados == 0) {
		strcpy(nombres, "No hay jugadores conectados.");
	} else {
		nombres[0] = '\0';
		for (int i = 0; i < lista->numConectados; i++) {
			strcat(nombres, lista->conectados[i].nombre);
			if (i < lista->numConectados - 1) 
				strcat(nombres, ",");
		}
	}
	pthread_mutex_unlock(&mutex);
}

void EnviarListaConectados(ListaConectados *listaConectadosPtr) {
	char listaUsuarios[512] = "8/";
	
	pthread_mutex_lock(&mutex);
	for (int j = 0; j < listaConectadosPtr->numConectados; j++) {
		strcat(listaUsuarios, listaConectadosPtr->conectados[j].nombre);
		if (j < listaConectadosPtr->numConectados - 1)
			strcat(listaUsuarios, ",");
	}pthread_mutex_unlock(&mutex);
	
	pthread_mutex_lock(&mutex);
	for (int j = 0; j < listaConectadosPtr->numConectados; j++) {
		write(listaConectadosPtr->conectados[j].socket, listaUsuarios, strlen(listaUsuarios));
	}
	pthread_mutex_unlock(&mutex);
}
void *AtenderCliente(void *socket)
{
    int sock_conn;
    int *s;
    s = (int *)socket;
    sock_conn = *s;

	int miSocket = DameSocket(sock_conn);
	if (miSocket == -1) {
		printf("No se ha encontrado el socket indicado\n");
		close(sock_conn);
		pthread_exit(NULL);
	}
	
    char peticion[512];
    char respuesta[512];
    int ret;
    int terminar = 0;
	char contrasena[20] = "";  // Almacena el nombre del usuario
	
	MYSQL *conn = mysql_init(NULL);
	if (!conn) {
		fprintf(stderr, "Error al crear la conexi�n MySQL\n");
		close(sock_conn);
		pthread_exit(NULL);
	}
	
	if (!mysql_real_connect(conn, "localhost", "root", "mysql", "chat", 0, NULL, 0)) {
		fprintf(stderr, "Error al conectar con MySQL: %s\n", mysql_error(conn));
		mysql_close(conn);
		close(sock_conn);
		pthread_exit(NULL);
	}

    while (terminar == 0)
    {
        ret = read(sock_conn, peticion, sizeof(peticion));
        if (ret <= 0)
            break;

        peticion[ret] = '\0';
        printf("Peticion: %s\n", peticion);

        char *p = strtok(peticion, "/");
        int codigo = atoi(p);
		char usuario[20], contrasena[50], nombre[20];

        if ((codigo >= 1 && codigo <= 3) || codigo == 5|| codigo == 6)
        {
            p = strtok(NULL, "/");
            strcpy(nombre, p);
            printf("Codigo: %d, Nombre: %s\n", codigo, nombre);
        }

        if (codigo == 0)
        {
			char mensaje[256];
			sprintf(mensaje, "5/%s se ha desconectado", nombre);  // 'nombre' es el usuario desconectado
			
			pthread_mutex_lock(&mutex);
			for (int j = 0; j < i; j++) {
				if (sockets[j] != sock_conn && autenticados[j] == 1) {  // No enviarlo al que se desconecta
					write(sockets[j], mensaje, strlen(mensaje));
				}
			}
			autenticados[miSocket] = 0;			
			pthread_mutex_unlock(&mutex);
					
			// Luego cerramos la conexión del cliente
			terminar = 1;
        }
        else if (codigo == 1)
        {

			p = strtok(NULL, "/");
			strcpy(contrasena, p);
			int login_ok = login(conn, nombre, contrasena);
			sprintf(respuesta, "1/%d", login_ok);
			write(sock_conn, respuesta, strlen(respuesta));
			
			if (login_ok == 1) {
				
				// Notificar a los demas que se ha conectado
				char mensaje[256];
				autenticados[miSocket] = 1;	
				sprintf(mensaje, "5/%s se ha conectado", nombre);
				
				pthread_mutex_lock(&mutex);
				for (int j = 0; j < i; j++) {
					if (sockets[j] != sock_conn && autenticados[j] == 1)  // No se lo enviamos al que se conecta
						write(sockets[j], mensaje, strlen(mensaje));
				}
				pthread_mutex_unlock(&mutex);
			}
			
        }
        /*else if (codigo == 2)
        {
            if (nombre[0] == 'M' || nombre[0] == 'S')
                strcpy(respuesta, "2/SI");
            else
                strcpy(respuesta, "2/NO");
        }
        else if (codigo == 3)
        {
            p = strtok(NULL, "/");
            float altura = atof(p);
            if (altura > 1.70)
                sprintf(respuesta, "3/%s: eres alto", nombre);
            else
                sprintf(respuesta, "3/%s: eres bajo", nombre);
        }*/

		else if (codigo == 5  && autenticados[miSocket] == 1)
		{
			p = strtok(NULL, "/"); // Obtener el mensaje del cliente
			if (p == NULL) {
				printf("Mensaje no valido\n");
				continue;
			} // Obtener hora actual del sistema
			time_t t = time(NULL);
			struct tm tm = *localtime(&t);
			char hora[16];
			strftime(hora, sizeof(hora), "%H:%M:%S", &tm); // Formatear mensaje con hora incluida
			char mensaje[512];
			sprintf(mensaje, "5/%s (%s): %s", nombre, hora, p);
			// Enviar a todos los clientes conectados
			pthread_mutex_lock(&mutex);
			for (int j = 0; j < i; j++)
				write(sockets[j], mensaje, strlen(mensaje));
			pthread_mutex_unlock(&mutex);
		}
		else if (codigo == 6)
		{
			p = strtok(NULL, "/");
			strcpy(contrasena, p);
			
			pthread_mutex_lock(&mutex);
			int res = register_player(conn, nombre, contrasena);
			pthread_mutex_unlock(&mutex);
			
			sprintf(respuesta, "6/%d", res);
			write(sock_conn, respuesta, strlen(respuesta));
		}
		else if (codigo == 7)
		{
			p = strtok(NULL, "/");
			if (p == NULL) {
				sprintf(respuesta, "7/0");
				write(sock_conn, respuesta, strlen(respuesta));
				continue;
			}
			strcpy(nombre, p); 
			p = strtok(NULL, "/");
			if (p == NULL) {
				sprintf(respuesta, "7/0");
				write(sock_conn, respuesta, strlen(respuesta));
				continue;
			}
			strcpy(contrasena, p);
			printf("[Eliminar] Usuario: %s, Contraseña: %s\n", nombre, contrasena);
			
			int login_ok = login(conn, nombre, contrasena);
			printf("login_ok = %d\n", login_ok);
			if (login_ok == 1)
			{
				int eliminado = eliminar_jugador(conn, nombre);
				sprintf(respuesta, "7/%d", eliminado);
				printf("Resultado de eliminar: %d\n", eliminado);
				
				if (eliminado == 1) {
					pthread_mutex_lock(&mutex);
					autenticados[miSocket] = 0;
					
					char aviso[256];
					sprintf(aviso, "5/El usuario %s ya no existe", nombre);  for (int j = 0; j < i; j++) {
						if (j != miSocket && autenticados[j] == 1 && sockets[j] > 0) {
							int enviado = write(sockets[j], aviso, strlen(aviso));
							if (enviado <= 0) {
								printf("Error al enviar mensaje a socket %d\n", sockets[j]);
							}
						}
					}
					pthread_mutex_unlock(&mutex);
				}
			}
			else
			{
				sprintf(respuesta, "7/0");
			}			
			write(sock_conn, respuesta, strlen(respuesta));	
		}

		/*else if (codigo == 8 && autenticados[miSocket] == 1) {
			char lista[512] = "8/";
			pthread_mutex_lock(&mutex);
			for (int j = 0; j < listaConectados->numConectados; j++) {
				strcat(lista, listaConectados->conectados[j].nombre);
				if (j < listaConectados->numConectados - 1)
					strcat(lista, ",");
			}
			pthread_mutex_unlock(&mutex);
			
			write(sock_conn, lista, strlen(lista));
		}*/
		else if (codigo == 9) {
			p = strtok(NULL, "/");  // usuario actual
			char usuario_actual[50];
			strcpy(usuario_actual, p);
			
			p = strtok(NULL, "/");  // nuevo nombre
			char nuevo_nombre[50];
			strcpy(nuevo_nombre, p);
			int resultado = ModificarNombre(conn, usuario_actual, nuevo_nombre);
			sprintf(respuesta, "9/%d", resultado);
			
			if (resultado == 1) {
				pthread_mutex_lock(&mutex);
				//strcpy(nombre, nuevo_nombre); // actualiza el nombre local en este hilo
				autenticados[miSocket] = 1;
				char aviso[256];
				sprintf(aviso, "5/%s ahora se llama %s", usuario_actual, nuevo_nombre);
				
				for (int j = 0; j < i; j++) {
					if (autenticados[j] == 1 && sockets[j] != sock_conn) {
						write(sockets[j], aviso, strlen(aviso));
					}
				}
				
				pthread_mutex_unlock(&mutex);
			}
			write(sock_conn, respuesta, strlen(respuesta));
		}
        if (codigo != 0 && codigo != 5 && codigo != 1 && codigo != 6)
        {
            pthread_mutex_lock(&mutex);
            contador++;
            char notificacion[20];
            sprintf(notificacion, "4/%d", contador);
			for (int j = 0; j < i; j++) {
				if (autenticados[j] == 1)
					write(sockets[j], notificacion, strlen(notificacion));
			}
            pthread_mutex_unlock(&mutex);
        }
    }

    close(sock_conn);
    return NULL;
}

int main(int argc, char *argv[])
{
    int sock_conn, sock_listen;
    struct sockaddr_in serv_adr;

    if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0)
        printf("Error creant socket");

    memset(&serv_adr, 0, sizeof(serv_adr));
    serv_adr.sin_family = AF_INET;
    serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
    serv_adr.sin_port = htons(9017);

    if (bind(sock_listen, (struct sockaddr *)&serv_adr, sizeof(serv_adr)) < 0)
        printf("Error al bind");

    if (listen(sock_listen, 3) < 0)
        printf("Error en el Listen");

    contador = 0;
    pthread_t thread;
    i = 0;

    for (;;)
    {
        printf("Escuchando\n");

        sock_conn = accept(sock_listen, NULL, NULL);
        printf("He recibido conexion\n");

        sockets[i] = sock_conn;
		autenticados[i] = 0;
        pthread_create(&thread, NULL, AtenderCliente, &sockets[i]);
        i++;
    }
}

int login(MYSQL *conn, char *id_usuario, char *contrasena){
	char consulta[300];
	int err;
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	printf("Login para eliminar: usuario=%s contraseña=%s\n", id_usuario, contrasena);
	
	strcpy(consulta, "SELECT nombre FROM usuarios WHERE id_usuario = '");
	strcat(consulta, id_usuario);
	strcat(consulta, "' AND contrasena = '");
	strcat(consulta, contrasena);
	strcat(consulta, "'");
	
	err = mysql_query(conn, consulta);
	if (err != 0) {
		printf("Error al consultar la base de datos: %s\n", mysql_error(conn));
		return 0;
	}
	
	resultado = mysql_store_result(conn);
	row = mysql_fetch_row(resultado);
	
	if (row != NULL) {
		mysql_free_result(resultado);
		return 1;
	}
	
	mysql_free_result(resultado);
	return 0;
}

int register_player(MYSQL *conn, char *nuevo_id, char *contrasena){
	char consulta[300];
	int err;
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	printf("1");
	// Verificar si el usuario ya existe
	strcpy(consulta, "SELECT id_usuario FROM usuarios WHERE id_usuario = '");
	strcat(consulta, nuevo_id);
	strcat(consulta, "'");
	
	err = mysql_query(conn, consulta);
	if (err != 0) {
		printf("Error al consultar la base de datos: %s\n", mysql_error(conn));
		return 0;
	}
	
	resultado = mysql_store_result(conn);
	row = mysql_fetch_row(resultado);
	
	if (row != NULL) {
		mysql_free_result(resultado);
		return 0;
	}
	
	mysql_free_result(resultado);
	
	// Insertar nuevo usuario
	strcpy(consulta, "INSERT INTO usuarios (id_usuario, contrasena) VALUES ('");
	strcat(consulta, nuevo_id);
	strcat(consulta, "', '");
	strcat(consulta, contrasena);
	strcat(consulta, "')");
	
	err = mysql_query(conn, consulta);
	if (err != 0) {
		printf("Error al registrar el usuario: %s\n", mysql_error(conn));
		return 0;
	}
	
	return 1;
}

int eliminar_jugador(MYSQL *conn, char *id_usuario){
	char consulta[300];
	int err;
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	
	// Verificar si el usuario existe
	strcpy(consulta, "SELECT id_usuario FROM usuarios WHERE id_usuario = '");
	strcat(consulta, id_usuario);
	strcat(consulta, "'");
	
	err = mysql_query(conn, consulta);
	if (err != 0) {
		printf("Error al consultar la base de datos: %s\n", mysql_error(conn));
		return 0;
	}
	
	resultado = mysql_store_result(conn);
	row = mysql_fetch_row(resultado);
	
	if (row == NULL) {
		// Usuario no existe
		mysql_free_result(resultado);
		return 0;
	}
	
	mysql_free_result(resultado);
	
	// Eliminar mensajes del usuario
	sprintf(consulta, "DELETE FROM mensajes WHERE emisor_id = '%s'", id_usuario);
	err = mysql_query(conn, consulta);
	if (err != 0) {
		printf("Error al eliminar mensajes: %s\n", mysql_error(conn));
		return 0;
	}
	// Eliminar el usuario
	strcpy(consulta, "DELETE FROM usuarios WHERE id_usuario = '");
	strcat(consulta, id_usuario);
	strcat(consulta, "'");
	
	err = mysql_query(conn, consulta);
	if (err != 0) {
		printf("Error al eliminar el usuario: %s\n", mysql_error(conn));
		return 0;
	}
	
	return 1;
}

int ModificarNombre(MYSQL *conn, char *usuario_actual, char *nuevo_nombre) {
	char consulta[300];
	int err;
	
	// Cambiar el nombre del usuario
	sprintf(consulta, "UPDATE usuarios SET id_usuario = '%s' WHERE id_usuario = '%s'", nuevo_nombre, usuario_actual);
	
	err = mysql_query(conn, consulta);
	if (err != 0) {
		printf("Error al actualizar nombre de usuario: %s\n", mysql_error(conn));
		return 0;
	}
	
	if (mysql_affected_rows(conn) == 0) {
		printf("El usuario '%s' no existe.\n", usuario_actual);
		return 0;
	} return 1;
}

int DameSocket(int sock_conn) {
	pthread_mutex_lock(&mutex);
	for (int j = 0; j < i; j++) {
		if (sockets[j] == sock_conn) {
			pthread_mutex_unlock(&mutex);
			return j;
		}
	}
	pthread_mutex_unlock(&mutex);
	return -1;
}
