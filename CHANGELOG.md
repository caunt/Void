# Changelog

## [0.0.2](https://github.com/caunt/Void/compare/v0.0.2...v0.0.2) (2025-04-14)


### Features

* **AbstractRegistryService:** enhance stream handling ([7835c99](https://github.com/caunt/Void/commit/7835c996e0b8e740c0275a030025d1dbf9cfcecd))
* **actions:** prerelease versioning ([941f0bd](https://github.com/caunt/Void/commit/941f0bd8807a1b618440599484af841c46010774))
* **actions:** test update ([55cc80d](https://github.com/caunt/Void/commit/55cc80d0f138244ed0cd954344f15ca387f4ea23))
* **actions:** test update ([4bd2095](https://github.com/caunt/Void/commit/4bd2095668366020fde05bb790b3cfdd05cd5aea))
* **actions:** test update ([ef4bd71](https://github.com/caunt/Void/commit/ef4bd714580ecdfe64154c1a229c2a0fde3e1f32))
* **actions:** test update ([09063b5](https://github.com/caunt/Void/commit/09063b54b63df28135dfd29ceaac1f7f25a8ce10))
* **actions:** test update ([5c7ce73](https://github.com/caunt/Void/commit/5c7ce732414f2b05b0216c32e0ab41e5dd7b6847))
* **attributes:** add configuration attributes and improve serialization ([477a3b1](https://github.com/caunt/Void/commit/477a3b1e46ab0920a91280b71c887e09e7613b9a))
* **authentication:** enhance message handling and packet limit ([13b6839](https://github.com/caunt/Void/commit/13b6839b6d8608397dfad6efa39f0cc18f994f64))
* **authentication:** improve server compatibility for players ([bb77b25](https://github.com/caunt/Void/commit/bb77b25d39187926a0fce81d150ea9436e3280aa))
* **authentication:** update protocol version handling ([5bf2858](https://github.com/caunt/Void/commit/5bf285846a4853a811c1f0187767da9335d430fd))
* base transformation property types ([#3](https://github.com/caunt/Void/issues/3)) ([9b84973](https://github.com/caunt/Void/commit/9b849737ca44be7620e54fbf6dd8d1a8aeb82fc6))
* **buffer:** implement IMinecraftBuffer interface and extensions ([30aef7c](https://github.com/caunt/Void/commit/30aef7c8cf1f0b04a45089f9ddda6e3fd90516b8))
* **buffer:** introduce BufferMemory and BufferSpan structs ([fc781b7](https://github.com/caunt/Void/commit/fc781b7aaee85a51c4e2e5f23a00a804db418da7))
* **buffers:** add BufferStream struct in new namespace ([db12623](https://github.com/caunt/Void/commit/db1262329641be390bb3ecfafd0d2af0eb2a588f))
* **buffers:** enhance reading and writing methods ([2ecac77](https://github.com/caunt/Void/commit/2ecac7774127fba2e0fb72e390ea711d7c4ed737))
* **ChatMessagePacket:** update message handling to use string ([0fc01e3](https://github.com/caunt/Void/commit/0fc01e39997d0744645f0581148a48ae7fee1664))
* **chat:** update ChatMessagePacket for protocol changes ([8aba07f](https://github.com/caunt/Void/commit/8aba07fdd3a8534d1700e5e6e0a1e2a8e7464908))
* **colors:** add color downsampling and serializer updates ([96e0b65](https://github.com/caunt/Void/commit/96e0b65a739575e5f95345ed37870b2da2f5572e))
* **common:** restructure networking interfaces and add features ([38f732f](https://github.com/caunt/Void/commit/38f732f4434543cdfd92898ec7a894451d661a64))
* **component:** add AsText property and ReadFrom method ([18f8400](https://github.com/caunt/Void/commit/18f8400951a282e9e4cdf93469a72a8787aa0742))
* **component:** add buffer serialization methods ([663474b](https://github.com/caunt/Void/commit/663474bb5486cec9880113dfb925b8794138603c))
* **component:** enhance serialization methods and documentation ([6cdd9c4](https://github.com/caunt/Void/commit/6cdd9c482bd0c76da9c92b590a90302d236eb580))
* **config:** add async methods for configuration retrieval ([6590cb7](https://github.com/caunt/Void/commit/6590cb7782196d90eba06d1f9923902253a5d25f))
* **config:** add IConfiguration and IConfigurationService interfaces ([a31c7a3](https://github.com/caunt/Void/commit/a31c7a3fe77748c5dcf71a87967deed06da96775))
* **config:** add plugin support to IConfigurationService ([978e99f](https://github.com/caunt/Void/commit/978e99f047567905d74b49fa86f4d74d393ac4ba))
* **config:** enhance IConfigurationService with event handling ([85dace4](https://github.com/caunt/Void/commit/85dace4ff826567de33494a7b2e2e2ff7118492c))
* **config:** refactor configuration handling and loading ([e549bf2](https://github.com/caunt/Void/commit/e549bf28b5ca92cebe75aeebb347eb72f581b7bb))
* **configuration:** enhance serialization and type handling ([f16f385](https://github.com/caunt/Void/commit/f16f385276e038a8535bc2418e6555dd6dd10d6c))
* **configuration:** implement timer-based monitoring ([872c948](https://github.com/caunt/Void/commit/872c9481c2f2c812250c0a960884a96b8a86a7de))
* **configurations:** add ConfigurationNameAttribute class ([f455771](https://github.com/caunt/Void/commit/f4557710e9839692c1e4a415d5508d329809ad8c))
* **configurations:** add ConfigurationService class ([9b1e73b](https://github.com/caunt/Void/commit/9b1e73b63d2812db3939a789ff957ed4cf72b76e))
* **ConfigurationService:** add skippedUpdates for efficiency ([5572de6](https://github.com/caunt/Void/commit/5572de6121eec4b39aedc887c3d24d70699223d6))
* **ConfigurationService:** enhance logging and file watcher ([0270618](https://github.com/caunt/Void/commit/0270618bd5ae4eb7bff139cc2c254a39c7afccf1))
* **ConfigurationTomlSerializer:** enhance TOML mapping and swapping ([aae43b6](https://github.com/caunt/Void/commit/aae43b6ca4c06a390d3d3fd7cef65f5d41c41659))
* **deps:** add TypeExtender package reference ([93e027f](https://github.com/caunt/Void/commit/93e027f81c2eddd10ba60ac084456aab30a25b8f))
* **docker:** update container message and add env variable ([981b9c5](https://github.com/caunt/Void/commit/981b9c51fbe2aa954b750f14f79d20130265b269))
* **docker:** update imageTag logic for Minecraft versions ([d1162fd](https://github.com/caunt/Void/commit/d1162fd42d9576db76f89f258ec77a90c92035ad))
* **docker:** update Minecraft server version and type ([1630312](https://github.com/caunt/Void/commit/16303120eedd36a6d7840175073e9d2c36ba0235))
* **editorconfig:** enforce newline at end of files ([cabc1bf](https://github.com/caunt/Void/commit/cabc1bf80faa029857ac8b4b2cf65a504c84d3d5))
* enhance Minecraft protocol handling and transformations ([9f4154c](https://github.com/caunt/Void/commit/9f4154cb2bbb0d2c50c1d324635a79787b89b3f9))
* **entrypoint:** add configuration service support ([4e9bb2a](https://github.com/caunt/Void/commit/4e9bb2a2938104acede4a77193d189907ebdf573))
* **EventService:** enhance cancellable parameters creation ([be5d2ac](https://github.com/caunt/Void/commit/be5d2ac9974a3f2d2acbf2ee5a2398f4120a00c1))
* **EventService:** enhance event handling with lifecycle support ([7fb8de0](https://github.com/caunt/Void/commit/7fb8de0ee47ef7b10be8f717ee1bca8a650a2650))
* **EventService:** improve parameter type compatibility check ([f486532](https://github.com/caunt/Void/commit/f4865320f5f1967f9bc5bd101f9d0e892ae3873a))
* **exceptions:** add InvalidConfigurationException class ([b4fd0ee](https://github.com/caunt/Void/commit/b4fd0ee5fce67bf0a30792a15f1bebf5ef565c48))
* **exceptions:** introduce StreamClosedException for stream handling ([c744b86](https://github.com/caunt/Void/commit/c744b86fc73cc540c7182516e61d2663c893289b))
* **extensions:** add IntExtensions class for VarInt handling ([fdf75cc](https://github.com/caunt/Void/commit/fdf75ccbebf81e90f6abd0917fcd9309f3531eb6))
* force named nbt in named nbt property ([a977ea4](https://github.com/caunt/Void/commit/a977ea4b5426e8927748057acaaa84f008d0a4ce))
* **interface:** add new methods and property to IMinecraftBinaryPacketWrapper ([81ab6c3](https://github.com/caunt/Void/commit/81ab6c30ee55f57df9d1ccee0dbd88e847435d48))
* **IntExtensions:** add EnumerateVarInt method ([da64c74](https://github.com/caunt/Void/commit/da64c74be94929d417e7f75e9ed78b646b0de202))
* **lifecycle:** add checks for player and protocol version ([5033146](https://github.com/caunt/Void/commit/50331462e2b9e6cfc2503d769bd2d43540ada5e6))
* **link:** add destructor and improve lock usage ([84da14b](https://github.com/caunt/Void/commit/84da14ba932a5ec4212d11d9a6292be3d745d710))
* **link:** enhance task management during stopping process ([91e45d0](https://github.com/caunt/Void/commit/91e45d0d38a558aa96e0e2b337f6045ce4bd7b25))
* **logging, packet:** update logging level and enhance dimension data handling ([a0565a3](https://github.com/caunt/Void/commit/a0565a36bf2525bda6013380c9c3645ecf06a351))
* **logging:** enhance error handling and connection logging ([a77b9d5](https://github.com/caunt/Void/commit/a77b9d51f32c5a287806d61d755dbd0afb38759d))
* **message-handling:** refactor to use INetworkMessage interface ([83de631](https://github.com/caunt/Void/commit/83de631c890f2b6ba7245633eeb99e495bc055f7))
* **MessageReceivedEvent:** add cancellation support ([b98515d](https://github.com/caunt/Void/commit/b98515d807288c5af04251b26404c9f2f2b38a31))
* **MinecraftBuffer:** add Dump method for buffer snapshot ([14713b8](https://github.com/caunt/Void/commit/14713b8e82bb96973a3d2044ea706f4e334f3a00))
* **minecraft:** update protocol version and Docker setup ([a37a88e](https://github.com/caunt/Void/commit/a37a88e689c56b753e527b63756461a29ccdb2a8))
* **minecraft:** update protocol version and refactor method call ([5280bf6](https://github.com/caunt/Void/commit/5280bf6e170cb0cb5948a593dbb7d6b4f388d276))
* named nbt tags ([82219fc](https://github.com/caunt/Void/commit/82219fcffec3420d51f6245edad838d89d3e153f))
* **namespace:** refactor and modularize network structure ([45356b5](https://github.com/caunt/Void/commit/45356b5c6064e7adf7a430561b86b2c4b6e1fd82))
* **nbt:** add implicit conversions between NbtBoolean and NbtByte ([06a65bc](https://github.com/caunt/Void/commit/06a65bc3634c3c5daa528ae7de14cedd5662f8b5))
* **nbt:** add implicit conversions for NbtBoolean ([28216ae](https://github.com/caunt/Void/commit/28216aec7300e82b9097bc95a57cc7d7cc7ad4c8))
* **nbt:** add NamedNbtProperty and enhance NbtProperty ([cfe3fc2](https://github.com/caunt/Void/commit/cfe3fc2b6e502df268d1e514064b6600556e765e))
* **nbt:** enhance NbtTag with buffer reading capabilities ([287b277](https://github.com/caunt/Void/commit/287b277bea64ea0c5a36a5e72fa2e86f3b947a37))
* **NbtTag:** refactor AsStream and AsJsonNode methods ([6e5c39c](https://github.com/caunt/Void/commit/6e5c39cc9b02b0a822e76e6e78c2f18eba908625))
* **network:** add Side parameter to message handling ([100a2cb](https://github.com/caunt/Void/commit/100a2cbdd3a9e9495686fbd6a47f5f6bde7b76c3))
* **network:** refactor namespaces and add new interfaces ([3cdf567](https://github.com/caunt/Void/commit/3cdf567b12b08184597a211a9e16acd673f0e108))
* offile uuid support ([c3203e5](https://github.com/caunt/Void/commit/c3203e5ae65ed0c4dfa533da75861eb539917fb1))
* **packet:** add Checksum property to SignedChatCommandPacket ([e720547](https://github.com/caunt/Void/commit/e7205478c8df1162fca592fd36105af51dcdab44))
* **packet:** add constructor for ChatMessagePacket v1.20.2-3 ([a616a81](https://github.com/caunt/Void/commit/a616a81fb855c45ef770e39fc20c480c824448e1))
* **packet:** add transformation mappings for ChatMessagePacket ([aee53b1](https://github.com/caunt/Void/commit/aee53b1fbeadfaac05a341bb073b0a81204c948d))
* **packet:** update reducedDebugInfo handling ([3a83a4f](https://github.com/caunt/Void/commit/3a83a4f6e2906f3466d2caa6b2a1e69b3fc51407))
* **player-service:** add Minecraft player functionality ([4d219c4](https://github.com/caunt/Void/commit/4d219c49cca613c6b98fe2d15199f698f6fd3625))
* **plugins:** add ignoreEmpty parameter to LoadPluginsAsync ([9937b69](https://github.com/caunt/Void/commit/9937b690204629100bfdca01b41aa566f0f290c0))
* **plugins:** refactor plugin interfaces and services ([6ea6420](https://github.com/caunt/Void/commit/6ea6420512235a7dea1149b6524134221a94f80c))
* **program:** enhance version handling in Docker setup ([e141aeb](https://github.com/caunt/Void/commit/e141aebb6510e6869144219123dadabe34783e76))
* **program:** enhance version retrieval in Docker setup ([f3e6bee](https://github.com/caunt/Void/commit/f3e6bee745eb7d9c9cff19019fcaa4fa3087ba61))
* **program:** update default protocol version and add type ([1bae35c](https://github.com/caunt/Void/commit/1bae35c25674983fa166cb5f5a0ead52552ab5f8))
* **program:** update Minecraft version handling ([557d2d1](https://github.com/caunt/Void/commit/557d2d1dfe730b1b3bf59abe6eb42891d074b40a))
* **properties:** add BinaryProperty for handling byte data in packets ([db491f7](https://github.com/caunt/Void/commit/db491f7f3633ca5a5604b303d4f355596c49a0f4))
* **protocol:** add transformations for v1.7.6 to v1.8 ([26cf6e2](https://github.com/caunt/Void/commit/26cf6e297c5194520de4d22848bf0226c90e6820))
* **protocol:** correct version number for Minecraft 1.9.4 ([3092055](https://github.com/caunt/Void/commit/30920558cd6b89f6027a1d80f2689a00c86943f7))
* **protocol:** update chat message handling for new versions ([25cf813](https://github.com/caunt/Void/commit/25cf813983e2887a3a3911827790abb9e5ac851f))
* **protocol:** update Minecraft protocol to 1.11.1 ([9720468](https://github.com/caunt/Void/commit/972046839ffbeee234e1577c4484e878ff2c5d92))
* **protocol:** update Minecraft protocol version to 1.21 ([ec084d6](https://github.com/caunt/Void/commit/ec084d6eba4293d08beeef0c56cd3226506c6bf2))
* **protocol:** update Minecraft protocol versions and transformations ([8b811ae](https://github.com/caunt/Void/commit/8b811aefbbe8fece775a14888bd421e06504c4a8))
* **protocol:** update to latest Minecraft protocol version ([e57d9c0](https://github.com/caunt/Void/commit/e57d9c0339aff7e0deb8ba5d92f6e7062cf12e38))
* **protocol:** update to Minecraft 1.21.5 support ([63ab8a6](https://github.com/caunt/Void/commit/63ab8a6ec7770bc0d83ca174e05bedcb756b1cbf))
* **protocol:** update version check for chat message packet ([216f08e](https://github.com/caunt/Void/commit/216f08ebe364270f12dcdbebd038b683649ffb7a))
* read/write short ([93b70bc](https://github.com/caunt/Void/commit/93b70bcab0a1a86f648d46fa7a82f00a5399a69f))
* register transformators ([729e5af](https://github.com/caunt/Void/commit/729e5af3d105f348e500a0ec558b98f09ce2d557))
* **registry:** add IRegistryHolder interface and implementation ([84636a0](https://github.com/caunt/Void/commit/84636a086a568ec4b4eb35863fb85824fb92df95))
* **Registry:** add new chat message packet mappings ([97ff2a6](https://github.com/caunt/Void/commit/97ff2a6148a7fe2eb29a604f15d3221b5a20951e))
* **registry:** implement IRegistryHolder and update error messages ([aec211e](https://github.com/caunt/Void/commit/aec211e2d25f76fcaa7ffb38470e93815fc47f43))
* **serializer:** add IConfigurationSerializer and TOML support ([a4e0218](https://github.com/caunt/Void/commit/a4e021849c5450f7878e491dd1389985c8a65bcf))
* **serializer:** add JSON deserialization for Component ([6434b15](https://github.com/caunt/Void/commit/6434b1565e82ad72794fa9f40b5bf8288d55f5d1))
* **serializer:** add methods for object serialization ([e74d5cc](https://github.com/caunt/Void/commit/e74d5ccfee5c62e9b0c945ef8696121d52190101))
* **serializer:** add overloads for Serialize method ([19bde35](https://github.com/caunt/Void/commit/19bde35b019c347dfd5816385098f1a279876c51))
* **serializer:** enhance attribute handling in ConfigurationTomlSerializer ([4ac3419](https://github.com/caunt/Void/commit/4ac3419f503618edd67d92f1c37e73f18a88372a))
* **serializer:** enhance configuration serialization methods ([33d3b71](https://github.com/caunt/Void/commit/33d3b71dbd7474b9cdfc33b62cb75fa51cdbdac4))
* **serializer:** enhance dynamic type generation ([7ea423c](https://github.com/caunt/Void/commit/7ea423cb0c34355eed4c11f9e314dbf7735f42fe))
* **service:** enhance IConfigurationService for background tasks ([e9ee168](https://github.com/caunt/Void/commit/e9ee16824391e81f8e0e9fbb7f1ca0c8f98dfd19))
* **services:** add hosted services for Configuration and Platform ([d7f2ca4](https://github.com/caunt/Void/commit/d7f2ca4e2c38ea425e5063603944e93e697f8d40))
* **ShowEntity:** update parameter order and add ActionName ([123e7ae](https://github.com/caunt/Void/commit/123e7ae26c031223d9ac95ca72085998443fae59))
* **streams:** refactor to introduce IManualStream interface ([ff6aff5](https://github.com/caunt/Void/commit/ff6aff5514cb289b4bfdb553505ba8038aced0f3))
* **transformation:** replace NbtProperty with ComponentProperty ([49a3e80](https://github.com/caunt/Void/commit/49a3e8042c0f21b64cc09787411f37284ec74ca9))
* **transformations:** add packet transformation functionality ([1760199](https://github.com/caunt/Void/commit/1760199531e42d71f8f5c4c1db94efd26643f20e))
* **transformers:** add methods for version upgrades and downgrades ([267f59a](https://github.com/caunt/Void/commit/267f59a1ba562129d9c87b5e53206cf5309f747e))
* **transformers:** add version handling for JSON and NBT ([80e0c5d](https://github.com/caunt/Void/commit/80e0c5d1dd4ba9ebb54709cbda94fdaa1152a16d))
* **transformers:** add version passthrough methods ([8eee759](https://github.com/caunt/Void/commit/8eee759ad8857e9c94a7aa77d9d8c2400b24be9a))
* **transformers:** enhance Apply method for protocol versions ([e17a0ec](https://github.com/caunt/Void/commit/e17a0ec97ea59772636f845a4bca66e58d03e87f))
* **transformers:** enhance hoverEvent handling ([6c8d111](https://github.com/caunt/Void/commit/6c8d111e819478161a726fa5e21c0d8697984af3))
* **transformers:** enhance JSON and NBT component handling ([2a2cf6a](https://github.com/caunt/Void/commit/2a2cf6a707d4bea52aed958c720c7d1d7ea60835))
* **versioning:** target specific Minecraft version 1.21 ([6d1cace](https://github.com/caunt/Void/commit/6d1cace9b1ccef14c6934739a4223303200be90e))
* wrap packets on send ([750b01c](https://github.com/caunt/Void/commit/750b01c8109a650fd0cc0507cd60525e5b3541e2))


### Bug Fixes

* **actions:** syntax error ([04730ee](https://github.com/caunt/Void/commit/04730ee763ae13c97de82660e4efcc91fb670d94))
* **ChatMessagePacket:** make Sender property non-nullable ([c437042](https://github.com/caunt/Void/commit/c437042103d65c8f66b4402d446d9615639f9d7b))
* correct return statement in AsVarInt method ([c1d65b6](https://github.com/caunt/Void/commit/c1d65b648ce40b01519614029ca90a44e1109c6f))
* **link:** replace cancellationToken with forceCancellationToken ([48f9c37](https://github.com/caunt/Void/commit/48f9c376d701c3ac0caa6c63e8be435e39cd3f5e))
* **PlayerService:** update exception handling logic ([c5b58ce](https://github.com/caunt/Void/commit/c5b58cec826764c5092aa880e7410248494a9257))


### Performance Improvements

* optimize VarInt writing in buffers ([348767d](https://github.com/caunt/Void/commit/348767d76f34c2797c6d803b0355c6c0584584e9))
