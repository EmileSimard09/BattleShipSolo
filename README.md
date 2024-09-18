# ExpertProjet1

## Demande 1 - Gestion de la taille au début de la connexion

### Ajouts

Ajouts des méthodes suivants dans les classes de connexion pur faciliter l'envoie de confirmations (bool) et de taille (int)

        public async Task<bool> EnvoyerConf(bool conf)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(conf);
                byte[] laConf = Encoding.ASCII.GetBytes(jsonString);
                int bytesEnvoye = handleLaCo.Send(laConf);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> Recevoirconf()
        {
            try
            {
                byte[] bytes = new byte[64000];
                int bytesRecu = await handleLaCo.ReceiveAsync(new ArraySegment<byte>(bytes), SocketFlags.None);

                string data = Encoding.ASCII.GetString(bytes, 0, bytesRecu);
                bool taille = JsonSerializer.Deserialize<bool>(data);
                return taille;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

            public async Task<bool> EnvoyerTaille(int taille)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(taille);
                byte[] laConf = Encoding.ASCII.GetBytes(jsonString);
                int bytesEnvoye = client.Send(laConf);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public async Task<int> RecevoirTaille()
        {
            try
            {
                byte[] bytes = new byte[64000];
                int bytesRecu = await client.ReceiveAsync(new ArraySegment<byte>(bytes), SocketFlags.None);

                string data = Encoding.ASCII.GetString(bytes, 0, bytesRecu);
                int taille = JsonSerializer.Deserialize<int>(data);
                return taille;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }    
