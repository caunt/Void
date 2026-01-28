# Changelog

## [0.5.8](https://github.com/caunt/Void/compare/v0.5.7...v0.5.8) (2026-01-28)


### Bug Fixes

* **di:** ✨ skip activation of open generic services ([82603b8](https://github.com/caunt/Void/commit/82603b85191bf3582f03617a6376ae0179ab769b))

## [0.5.7](https://github.com/caunt/Void/compare/v0.5.6...v0.5.7) (2026-01-27)


### Features

* **auth:** ✨ handle early disconnects for status queries ([37a1839](https://github.com/caunt/Void/commit/37a183991b4c0c11d9873939f2ca2468dea1d5ca))
* **link:** ✨ improved player reconnection on failed connection requests ([cacd51a](https://github.com/caunt/Void/commit/cacd51a9cc55104dd7c2ddd8aa4281871b5c7ca6))
* **nuget:** ✨ added NuGet repository probing functionality ([c355700](https://github.com/caunt/Void/commit/c3557004a1cac191fbacc5876292998a2c5e77d9))
* **protocol:** ✨ expanded SetHeldItem packet definitions from 1.7.2 to latest ([9e9f2b4](https://github.com/caunt/Void/commit/9e9f2b4857fa1bc23095f57ecfddcec6be2ddc8a))
* **resolver:** ✨ improved assembly path resolution ([a2b0bed](https://github.com/caunt/Void/commit/a2b0bed59b8e576d5bff67e913e1fa7331632b8c))
* **server:** ✨ allow --server option without port, default to 25565 ([0eb44d9](https://github.com/caunt/Void/commit/0eb44d901f000a7763774df6bd77effc43f85238))
* **server:** allow --server option without port, default to 25565 ([8c6cc88](https://github.com/caunt/Void/commit/8c6cc8885693269279d92456baf84ef40945bcd1))


### Bug Fixes

* **common:** 🐛 corrected cancellation log spelling ([5612838](https://github.com/caunt/Void/commit/56128383c68f208eba805106a8e3ffb8561af30b))
* **essentials:** 🐛 corrected log message grammar ([742aa15](https://github.com/caunt/Void/commit/742aa1571bc3376e735b6c1703e5a3b493f90f3f))
* **keepalive:** 🐛 reset tracker on server redirection ([6d73acd](https://github.com/caunt/Void/commit/6d73acdc7e5662debb6c6915d6437d3f025da460))
* **kicks:** ✨ allowed changing phase to login without a link to kick players early ([b8df337](https://github.com/caunt/Void/commit/b8df3371ae36f943d12e9c2fafc995dbe1f41499))
* **lifecycle:** ✅ added IsMinecraft check in OnPlayerDisconnected ([6fecca8](https://github.com/caunt/Void/commit/6fecca8a71661a5398816e4247bf7fbe7253e0fd))
* **links:** 🐛 allow multi-server retry on connection failure ([7f13515](https://github.com/caunt/Void/commit/7f13515c95657ca94a921b46f04d1f0c93d9db99))
* **links:** 🐛 corrected timeout log wording ([5e2a687](https://github.com/caunt/Void/commit/5e2a6873e54a3312eebdd7c881095100b6c69e8d))
* **minecraft:** 🐛 correct XML doc langword typo ([0d40cf8](https://github.com/caunt/Void/commit/0d40cf84cc79c6884c0bdad6be1ce8c51d79b1fa))
* **nbt:** 🐛 corrected network format description ([6ec23d3](https://github.com/caunt/Void/commit/6ec23d3918b6e99969a503c38ec9e705d10e256c))
* **nuget:** ✨ improved custom repository handling and error logging ([61a70c5](https://github.com/caunt/Void/commit/61a70c50a900ac4d71a8d55a0273bfc095184f09))
* **nuget:** 🐛 declare sanitizedUrl once and assign appropriately ([0b87567](https://github.com/caunt/Void/commit/0b87567c40939cb4b412869f7897d812bbb1230f))
* **nuget:** 🐛 initialize sanitizedUrl once, reassign only for valid URIs ([2147d98](https://github.com/caunt/Void/commit/2147d98baaa2ec95dcd55e2693b6444898667cac))
* **nuget:** 🐛 removed duplicate sanitizedUrl reassignment ([7d725fe](https://github.com/caunt/Void/commit/7d725fe37dee358ba999a1c4030facfa0d1d3c23))
* **nuget:** 🔐 keep credentials in HTTP probe requests ([280e8eb](https://github.com/caunt/Void/commit/280e8eb220f6310dd04561666ec1293e5d2f025d))
* **nuget:** 🔒 added sanitized URI to invalid repository logging ([72a47d0](https://github.com/caunt/Void/commit/72a47d014e5a7fa5a15472c6ff7a84e61f238529))
* **nuget:** log error status code instead of general failure ([4e622c3](https://github.com/caunt/Void/commit/4e622c3a42eac1f885ab5326aa60c0f804360b0a))
* **player-service:** 🐛 updated error messages for server connection failures ([0a6a5ef](https://github.com/caunt/Void/commit/0a6a5ef00ae4ba109cf59d0b45d53cd63bd0ca90))
* **player:** 🐛 unwrap player before kick operations ([1ce1b16](https://github.com/caunt/Void/commit/1ce1b16ab71973d5b9cf1888b03bb0b4c36f41c7))
* **plugins:** 🐛 honor remote plugin URLs ([525a456](https://github.com/caunt/Void/commit/525a4567c12cdd1b2d9c6e132d1a77be0aaf8904))
* **plugins:** 🔇 set HttpClient log level to Warning to suppress verbose logs ([2c62348](https://github.com/caunt/Void/commit/2c623486b26baf975e53c2c6ace94eefa0123bdd))
* **plugins:** 🔇 suppress verbose HttpClient logs and show full URI in errors ([9b4db14](https://github.com/caunt/Void/commit/9b4db146353429d17c68daa28bfdaf409db774be))
* **protocol:** 🐛 logged actual player instance instead of TcpClient ([9780a94](https://github.com/caunt/Void/commit/9780a94f5f40ff3ad6783c3fa2d5b711e898e541))
* **resolver:** 🛠️ handle missing plugins directory safely ([d8af77e](https://github.com/caunt/Void/commit/d8af77e431b4cb51ddbf1c01903c79ed40b8e7b2))


### Reverts

* remove last two rebase-related commits ([e317f8e](https://github.com/caunt/Void/commit/e317f8ed59020e7c3769e31be3488a4cb7fa81fb))

## [0.5.6](https://github.com/caunt/Void/compare/v0.5.5...v0.5.6) (2026-01-12)


### Features

* **api:** ✨ added player name property ([361cb9b](https://github.com/caunt/Void/commit/361cb9b08d0abdc860695f6d97999172c7c02b05))
* **buffer:** ✨ added text components json serialization support for network buffers ([0edb11f](https://github.com/caunt/Void/commit/0edb11fe8ed7f1a2899b126a7af342dcfad136dc))
* **buffer:** ✨ unified component (de)serialization API ([c840c21](https://github.com/caunt/Void/commit/c840c21e7cc01dc1d6b6a949f8b40d17870a1a21))
* **buffers:** ✨ added support for large buffer indices ([2a96fb5](https://github.com/caunt/Void/commit/2a96fb5da5ffa1841cc1187fc26a2a79929d8a6c))
* **components:** ✨ added more control over tag name serialization ([058f8e0](https://github.com/caunt/Void/commit/058f8e006b00906b9a42302d80cf61862249c5a1))
* **components:** ✨ added support for literal content type ([8518d28](https://github.com/caunt/Void/commit/8518d2888af1970a7673705e9fa44c4a81329cf6))
* **core:** ✨ migrate some APIs to extension properties ([51ee50d](https://github.com/caunt/Void/commit/51ee50d205026aea1040b7638a160a3f50d05d02))
* **events:** ✨ added PlayerJoinedServerEvent when both sides in Play phase ([4528dae](https://github.com/caunt/Void/commit/4528daeefeb571fd345e22bdb7c257e8e7297ec3))
* **extensions:** ✨ added flexible packet transformation API ([cd8cb6a](https://github.com/caunt/Void/commit/cd8cb6a2cdae08a87d49e3493703736fb48fe1f5))
* **lifecycle:** ✨ added keep alive tracking and timeouts ([1be6f6f](https://github.com/caunt/Void/commit/1be6f6f0e7adb091f9edadc205c4876b29c210b7))
* **nbt:** ✨ added TryGetValue method to NbtCompound ([bceed05](https://github.com/caunt/Void/commit/bceed05cead311c14fb8f0bee21e0eb47bf0d60b))
* **nbt:** ✨ integrated archived Eric Freed's project SharpNBT ([91356b3](https://github.com/caunt/Void/commit/91356b3210957ce9e56ffd7dcd5a4613d07898c7))
* **network:** ♻️ centralized packet transformation registration ([8ff8dcd](https://github.com/caunt/Void/commit/8ff8dcdbab9c29296bd0a78cb612ec172bae4d8b))
* **network:** ✨ centralized packet id definitions ([682db46](https://github.com/caunt/Void/commit/682db46c60377cca1531d4b830e829aa1d18ac7f))
* **network:** ✨ implemented stream timeouts ([ad0253f](https://github.com/caunt/Void/commit/ad0253f9047c1972994f03e4f39ddf0f14aa0a40))
* **properties:** ✨ added more parsing capabilities ([f1bd643](https://github.com/caunt/Void/commit/f1bd643eb9d38de3e8bb18f9ea7611619b411316))
* **properties:** ✨ improved JSON serialization flexibility ([48ad114](https://github.com/caunt/Void/commit/48ad1146d436d63781d79865d5174ab1347da382))
* **serializer:** ✨ improved NBT tag type safety ([8e77609](https://github.com/caunt/Void/commit/8e776093fcc40f44abdea00023f3f96f8041921e))


### Bug Fixes

* **api:** 🐛 improved error handling and logging ([5bf37e9](https://github.com/caunt/Void/commit/5bf37e98c764dc8abdf647b08980c49924c8d02f))
* **auth:** 🐛 improved error handling for early closing connections ([873550e](https://github.com/caunt/Void/commit/873550ea98e08e3e354de5d193252b63bb8ae45d))
* **buffer:** 🐛 changed default value of writeName parameter to false for components ([756d97e](https://github.com/caunt/Void/commit/756d97e35aca6c78973772969a02e87c90c47159))
* **buffers:** 🐛 standardize buffer exception parameter order ([989a9e1](https://github.com/caunt/Void/commit/989a9e15130b6a7b4ea849039801604f743a95a5))
* **buffers:** 🛑 improved end-of-buffer error throwing ([da0ec0e](https://github.com/caunt/Void/commit/da0ec0e9ccca0ad869925a317216ea1fa3f36ed2))
* **command:** 🐛 improved unknown command handling ([0512629](https://github.com/caunt/Void/commit/0512629c0993cbd2366c20ef63ff5cbed86ce289))
* **command:** 🐛 prevented deadlock by not awaiting task from itself ([af68f35](https://github.com/caunt/Void/commit/af68f35f00f600f116f41339ae5ccf058a169498))
* **commands:** 🐛 ignore StreamClosedException in execution ([11ca15d](https://github.com/caunt/Void/commit/11ca15d4cc399a2eb7214d83e3b6c8c42ebc259c))
* **components:** ♻️ updated components to latest vanilla implementation ([8683da5](https://github.com/caunt/Void/commit/8683da51561f8326392d1fc6c212f69c10fdbd3c))
* **dependency:** 🐛 use cached player hash for lookups ([c88a0ac](https://github.com/caunt/Void/commit/c88a0acd15d6d13bdc9319b07ab2e329be6ed6c6))
* **exceptions:** ✨ improve exception messages with better context ([a2d2d98](https://github.com/caunt/Void/commit/a2d2d985883c6f1ce273ba94b4d629fd5347e4e4))
* **lifecycle:** 🐛 improved keep-alive handling and logging ([44958dc](https://github.com/caunt/Void/commit/44958dc7439c4c42e1bf71e46712358d5dfdfa20))
* **links:** 🐛 improved error handling in authentication ([74f118f](https://github.com/caunt/Void/commit/74f118fa14e8eb77800df7cc8f56d63c7796dc40))
* **logging:** 🐛 improved error handling for command executions ([aadae16](https://github.com/caunt/Void/commit/aadae1643357954b5acd817cff2489161cb85c19))
* **minecraft:** ✏️ correct tag parsing comment typo ([9552d8b](https://github.com/caunt/Void/commit/9552d8b9d36d9054238024e53ee0ad008b51631c))
* **nbt:** 🐛 allowed empty tag names like vanilla does ([cdf2a88](https://github.com/caunt/Void/commit/cdf2a885fbc4cd0b7d949aed0f53d8f206454303))
* **nbt:** 🐛 use Fields property instead of Values ([533908b](https://github.com/caunt/Void/commit/533908b99bb5dfe98052a70c66a0448fdda0c6e0))
* **nbt:** 🧹 removed unsigned type support from public API ([ce8402f](https://github.com/caunt/Void/commit/ce8402f7538f47e35a062394657ba312eba13981))
* **network:** ♻️ unified disconnect packet definition ([faea054](https://github.com/caunt/Void/commit/faea054f799eaebd3ef45b2b60e4ec8961bd5755))
* **network:** 🐛 reduced mods logging ([c2f235c](https://github.com/caunt/Void/commit/c2f235c7f11c84b2c9b47f685ef5194461774ba0))
* **network:** 🛠️ improved packet decoder method lookup ([17c788b](https://github.com/caunt/Void/commit/17c788b980e2bfa86202415dd7842002d1825fc5))
* **platform:** ♻️ renamed entry point for clarity ([b281541](https://github.com/caunt/Void/commit/b281541de1afdf539332753fa1dd71af8e1d70ae))
* **platform:** ✏️ correct suppression comment ([3a8d387](https://github.com/caunt/Void/commit/3a8d387c896d289df32b6c943303806650b8d4cd))
* **player:** 🐛 improved disconnect handling and logging ([6043c95](https://github.com/caunt/Void/commit/6043c95919dfbeb45b716628ccd1c7e725d23e03))
* **player:** 🐛 improved player disconnect event handling ([84c7db8](https://github.com/caunt/Void/commit/84c7db8e8fa25dd424f38b5f474c5a37ae866670))
* **player:** 🛠️ simplified player kick logic ([9c3887a](https://github.com/caunt/Void/commit/9c3887a5e32d2e43224269dc46fe7e0180eaf0c5))
* **protocol:** ✨ added keep-alive packet id mappings for all versions ([3d77afb](https://github.com/caunt/Void/commit/3d77afbeb6286fec81d5d3a1095a9e5d75c1d4ee))
* **protocolsupport:** ✏️ clarify login disconnect comment ([ceeafe8](https://github.com/caunt/Void/commit/ceeafe8909eceb166ec6007493a880d49c9698be))
* **protocolsupport:** 📝 correct authentication comment typo ([566b312](https://github.com/caunt/Void/commit/566b31286975b1a362b9be4f026dec172f0bb315))
* **registry:** 🛠️ skip empty packet transformation mappings ([87a434c](https://github.com/caunt/Void/commit/87a434ccc1c1ca6799604cdedf6864aa49bbaf13))
* **services:** 🛠️ improved error handling for disposed players context ([de974c8](https://github.com/caunt/Void/commit/de974c81b2cb5411302084b6b85faa99ad37d98e))
* **transformations:** 🛠️ register on server play phase only ([9bee10b](https://github.com/caunt/Void/commit/9bee10be6766afb9c96ff7268e2b7c2c8c807c39))


### Reverts

* **brigadier:** ⏪ revert exception improvements in CommandContext ([5d5536a](https://github.com/caunt/Void/commit/5d5536aeed0722ba42af2123e005cb9b7fab6e6e))

## [0.5.5](https://github.com/caunt/Void/compare/v0.5.4...v0.5.5) (2025-12-18)


### Features

* **api:** ✨ introduce run options interface and configuration ([0738af7](https://github.com/caunt/Void/commit/0738af778c9867378f5812cb579cd6dea1b00c7c))
* **auth:** ✨ improve authentication result feedback ([5ec8677](https://github.com/caunt/Void/commit/5ec8677d556bb08b74d8a8c39f8960d0e39b81fc))
* **authentication:** ✨ added login plugin message handling ([4cd1c2f](https://github.com/caunt/Void/commit/4cd1c2f96af0aafa59d41d99a460b6bd2d00732f))
* **buffer:** ✨ added more dump methods ([201bc51](https://github.com/caunt/Void/commit/201bc510c2b798b7d4c67b7e6f5a7793e637d2fc))
* **channels:** ✨ added Channels property to ILink to simplify enumeration over channels ([7e19e20](https://github.com/caunt/Void/commit/7e19e20f42bdc86f16917545c661bd2f9d14773f))
* **config:** ✏️ added read-only mode to disable saving changes to configuration files ([3018667](https://github.com/caunt/Void/commit/30186679f2b78b155b7aba74fd44232b85e883de))
* **console:** ✨ allow passing command-line options to plugins ([36944b5](https://github.com/caunt/Void/commit/36944b56738c6f7dc8b27b1f8dc4d1b10e5de8c7))
* **console:** ✨ manual command-line option discovery ([2e5124a](https://github.com/caunt/Void/commit/2e5124afa8367168f66d4e9c3c9ed22894db2579))
* **debug:** ✨ support forge servers in debug harness ([c28ceac](https://github.com/caunt/Void/commit/c28ceac1eaf907bf6fe083943b479a8b82cf7e47))
* **events:** ✨ added event waiting functionality ([edea1a9](https://github.com/caunt/Void/commit/edea1a9815ba029ee9e9053c0447908831db020d))
* **events:** ✨ added side to handshake event ([e61321b](https://github.com/caunt/Void/commit/e61321b88836e8bbd269629c7a54a25b6dfab76b))
* **forge:** ✨ added handling of forge handshake ([0661c26](https://github.com/caunt/Void/commit/0661c2653e7e1951718abef2f3d405a03a475372))
* **forge:** ✨ handle markers in handshake ([3bfee5d](https://github.com/caunt/Void/commit/3bfee5d573e3f03cae1ab29729fe6037686337ac))
* **forge:** ✨ listen to handshake packet ([b1e8d84](https://github.com/caunt/Void/commit/b1e8d84005db9a8a96934f8048d4e0b92e3ecd6d))
* **forge:** ✨ reuse cached client mods to allow fml servers redirections ([2d93c16](https://github.com/caunt/Void/commit/2d93c160af2be7a6803fbd203dd0c6e5b02ad9f3))
* **forwarding:** ✨ allow passing modern forwarding key by command-line option ([19257cc](https://github.com/caunt/Void/commit/19257cc55d0dc5908c02eb23d6977fa29e532ecd))
* **inventory:** ✨ add example support for 1.20/1.20.1 versions ([f93dc9c](https://github.com/caunt/Void/commit/f93dc9c9b04206864a0b3ed5fb24e804cacaa50a))
* **logging:** ✨ introduce dynamic logging level control ([53c6805](https://github.com/caunt/Void/commit/53c6805081629eac55df837b6e13aea5530e8df4))
* **logging:** 🔧 added logger to player scoped context ([45ee246](https://github.com/caunt/Void/commit/45ee2467b57f5ed9c816f1ec0b0d8f72f886a791))
* **options:** ✨ use working directory from run options ([4e40d25](https://github.com/caunt/Void/commit/4e40d254efafb1d605df31c206b957cdce61de22))
* **packets:** ✨ added play phase plugin message codec ([25c1d9e](https://github.com/caunt/Void/commit/25c1d9e29c467c4f7d0af4c50e61e2e16f0852b1))
* **player:** ✨ added ConnectedAt property ([c7268a8](https://github.com/caunt/Void/commit/c7268a89f98b48a52bdf6c2b34482a05482f1510))
* **players:** ✨ added traffic pause/resume api for players ([c8dc96f](https://github.com/caunt/Void/commit/c8dc96fb69fc16acbc32d55800a1482162b6b7f4))
* **protocol:** ✨ add support for 1.21.11 protocol version ([e0fddca](https://github.com/caunt/Void/commit/e0fddca736e2c2a4bc7e11e03dae3d5d0e20299e))
* **protocol:** ✨ add support for version 1.21.10 ([0ad76fd](https://github.com/caunt/Void/commit/0ad76fde472746d73bb557bf5064ed058ded98bc))
* **registries:** ✨ add direction-based clearing ([4cfc442](https://github.com/caunt/Void/commit/4cfc442e20f8a917d6448ca9d816e5370a1b6f71))
* **version:** ✨ support 1.21.9 minecraft ([7570d3a](https://github.com/caunt/Void/commit/7570d3a6e9d9e0534d529c9e55b8a491cd1bdf3d))


### Bug Fixes

* **auth:** 🐛 correct typo in server authentication comment ([18eb4e1](https://github.com/caunt/Void/commit/18eb4e1d983db2c93698b6072df795dc9e632b37))
* **auth:** 🐛 ensure non-null auth result default ([603453b](https://github.com/caunt/Void/commit/603453b5ea2b37d4400f8eb342034de98b15ccbf))
* **auth:** 📝 correct response wording ([bff47aa](https://github.com/caunt/Void/commit/bff47aa7383c2097ca78c8da552e4206e722d4b3))
* **auth:** 🛠️ only finalize server login on successful auth ([22e908c](https://github.com/caunt/Void/commit/22e908cda48879a8fe752850bdd9c1390d6227db))
* **auth:** 🛡️ finish login after login is completed on server ([502c319](https://github.com/caunt/Void/commit/502c319704cb54654efcbbc2e102f63b6aa750bd))
* **auth:** 🛡️ separate player and server auth results ([7ae7b2b](https://github.com/caunt/Void/commit/7ae7b2b20ec36ab83468b29153c067ffd9099c61))
* **authentication:** ✨ allow more types of packets at authentication ([5c2d07b](https://github.com/caunt/Void/commit/5c2d07b8eb2513465c0e8771f95c41518ee33766))
* **channels:** 🔧 throw channel created event after player context channel property is set ([048f474](https://github.com/caunt/Void/commit/048f474d59f0ef826561f2c7a28b1974059075ee))
* **chat:** 🔧 compare plugin with example plugin ([377e799](https://github.com/caunt/Void/commit/377e799b53af298e789984e3301dfb2883b3ca8b))
* **commands:** 🐛 improved command execution tracking ([3f3ce1f](https://github.com/caunt/Void/commit/3f3ce1f27183e5ff41e63b281d40ad66cca87175))
* **common:** 📝 correct channel registry comment ([bca0f9f](https://github.com/caunt/Void/commit/bca0f9f7e1e3d14b3956c2674ee439cafdbcd29c))
* **components:** 🐛 improve JSON deserialization logic ([2477dfe](https://github.com/caunt/Void/commit/2477dfeab3369c4600aadac020dad3e2499ae2c8))
* **config:** 🧹 ensure all plugin configurations are removed ([fdf5974](https://github.com/caunt/Void/commit/fdf5974c22b9e927874ae4221c3985723dbbe663))
* **console:** ♻️ remove setup method and update buffer for each read ([c9b4dd7](https://github.com/caunt/Void/commit/c9b4dd791fbfe4b64bcb8e7a0156c0f734aab587))
* **console:** ✨ check for options implicity ([0d42b15](https://github.com/caunt/Void/commit/0d42b155842ee76ae7f7298c8952bfecd6de1775))
* **console:** ✨ simplify command-line option detection ([969f0a1](https://github.com/caunt/Void/commit/969f0a1723fc66ecc35a7280e696c924f5807496))
* **console:** 🐛 remove unreachable null check ([12b5e7d](https://github.com/caunt/Void/commit/12b5e7d0b8db87c5c6a36d707c9bedecd122af66))
* **dependencies:** ✨ reimplement file dependency resolver to escape deps.json requirement ([6c96a28](https://github.com/caunt/Void/commit/6c96a2859ee1b100ecf5f7861dc5a8b785ac39bb))
* **dependencies:** 🛠️ improved assembly resolver path selection ([8f12339](https://github.com/caunt/Void/commit/8f123398799cadbd3da3dd17e47bb6eeaf1f65b3))
* **dependency-injection:** ✨ discover repository option early ([b6368b8](https://github.com/caunt/Void/commit/b6368b8c25bd5da09a2c18831050b57779a4f0ac))
* **di:** 🔧 use adapter for DryIoc DI rules ([80e67f3](https://github.com/caunt/Void/commit/80e67f36b69ff7d7b71de9b86fd9eceb46248ffe))
* **entry-point:** ♻️ optimize working directory handling ([f0c2c33](https://github.com/caunt/Void/commit/f0c2c33535a671f72b71c5df76a023bdc3667470))
* **entry-point:** 🐛 do not pass cancelled token ([b01225b](https://github.com/caunt/Void/commit/b01225bc99a8f0cf19c8dfe6192764f642ea699b))
* **essentials:** ✨ simplify server redirections ([78fd87b](https://github.com/caunt/Void/commit/78fd87b0a2c6e8750b4947222a410074a6efafaa))
* **events:** 🐛 enumerate all channels when rethrowing system packets ([0de8bbf](https://github.com/caunt/Void/commit/0de8bbf5a232b1a7f98b8b3a53793d32874c9da9))
* **example-plugin:** 📝 add missing article in comment ([235a79e](https://github.com/caunt/Void/commit/235a79ef56ca035938eb5bc1d709aac0a2cbfd1d))
* **example-plugin:** 📝 correct plugin load comment typos ([abc094b](https://github.com/caunt/Void/commit/abc094be164ff1a279c8bbd201cc006a31b0da1f))
* **forwarding:** 🔧 ensure forwarding option is discovered early ([4075c50](https://github.com/caunt/Void/commit/4075c50ff9d958ed9bdc41cce8d1594f14422e47))
* **forwarding:** 🔧 use selected secret key ([563b5ac](https://github.com/caunt/Void/commit/563b5acb7bc33dc8ef3bc62043aa80ea6f044ae1))
* **inventory:** 🛠️ update packet mappings to 1.21.9 ([c10af19](https://github.com/caunt/Void/commit/c10af1954d33bc66d50e57019fcf0b02f2b05694))
* **listener:** 🔧 change message processing order ([a1a57b1](https://github.com/caunt/Void/commit/a1a57b13520bed30e93cde1ea5c5d64d3d29d64f))
* **logging:** 🐛 improve error handling and logging ([7cc423c](https://github.com/caunt/Void/commit/7cc423c84070817930982dd3a7e5132212762ff7))
* **logging:** 🐛 improve log detail and direction enum usability ([2a29797](https://github.com/caunt/Void/commit/2a29797e27840c1455f10e53b8923d39f366c5a6))
* **logging:** 🐛 improve timeout error messages ([9dd5469](https://github.com/caunt/Void/commit/9dd546976b436eb8ea2a325b5409579de04165ee))
* **logging:** 🐛 improved server redirection logging ([1755d14](https://github.com/caunt/Void/commit/1755d14605e8f3260a2288ec309087850be57faa))
* **logging:** 🐛 include direction in error logs ([c083ed7](https://github.com/caunt/Void/commit/c083ed71a46232e40878e82b08db05056927bad7))
* **logging:** 🐛 streamline logging configuration ([8ac0937](https://github.com/caunt/Void/commit/8ac0937af00f68e170297c9dc1e467fa30baf9c2))
* **logging:** 🔧 add default case for packet logging ([33d8679](https://github.com/caunt/Void/commit/33d8679e64bf8195794b0fe248a875ff1e4f3c94))
* **logging:** 🔧 add trace for link stopping reason ([150d48f](https://github.com/caunt/Void/commit/150d48f722d39743b413976d06b96df467204a16))
* **logging:** 🔧 enhance log message with direction ([b7f6c4d](https://github.com/caunt/Void/commit/b7f6c4d166b06c51cc447715d35ae2bc1cf67c94))
* **logging:** 🔧 override microsoft logging level ([aae26de](https://github.com/caunt/Void/commit/aae26de6dfab8fb5a2dcee59d5eb5dc80712cd61))
* **logging:** 🔧 standardize logging across services ([3268348](https://github.com/caunt/Void/commit/326834867deecf99764ecea8eb7ab0752659a5e3))
* **logging:** 🔧 update plugin name in log message ([5a3efd4](https://github.com/caunt/Void/commit/5a3efd44c2f8e36c304fa5e01f6b91841077348b))
* **minecraft:** 🐛 correct typo in CommandDispatcher variable name ([5bc40d9](https://github.com/caunt/Void/commit/5bc40d9830fbe09d84cb03d8197e80cf3ac9f40c))
* **mods-support:** ✨ prepare for forge support plugin ([50460de](https://github.com/caunt/Void/commit/50460de853ba0b87d36e0e5f3f9e88ee1845da0b))
* **network:** 🐛 handle only valid sides & improve logging ([7145709](https://github.com/caunt/Void/commit/71457090367ede1b71836af1e68312b1b39f14f5))
* **network:** 🐛 specified exact operations to register on phase changes ([ae0bf7e](https://github.com/caunt/Void/commit/ae0bf7e266b678452ebc4df6160f692c4d25c73c))
* **network:** 🐛 suppress stream closed exception for kick api ([8d58334](https://github.com/caunt/Void/commit/8d58334583597b2dfd4e44aae207a5513e224564))
* **network:** 🔥 removed origin from channel APIs ([55104dd](https://github.com/caunt/Void/commit/55104ddd829a891e3f51b3ea869ce82bade2ff8b))
* **network:** 🔧 split plugin packet id registries by operation type ([a79b79e](https://github.com/caunt/Void/commit/a79b79e6e0689f32eb0c5915411c762109b05c75))
* **packets:** ✨ allow to set direction manually to register packets ([f140cfa](https://github.com/caunt/Void/commit/f140cfab007609dd9d626f8d823ab9434f7c2a7d))
* **platform:** 🐛 add null-safety checks for option aliases ([be31e65](https://github.com/caunt/Void/commit/be31e65a146c1594dbe413208583d58cfca08ada))
* **platform:** 🐛 parse interface option from string ([c7ebec4](https://github.com/caunt/Void/commit/c7ebec44338bc440b43af374f2e652d8762157e0))
* **platform:** 🐛 prevent collection modification during enumeration ([40a9900](https://github.com/caunt/Void/commit/40a9900cc3757e306e95460e1d50f7f6cd4b692c))
* **platform:** 📝 clarify file watcher comment ([b1973cb](https://github.com/caunt/Void/commit/b1973cba5e2805ebe64f886539ddcf42f0af099d))
* **platform:** 🔒 use AsyncLock for thread-safe option discovery ([ba6d86d](https://github.com/caunt/Void/commit/ba6d86ddc99629343f081cd6d1492c71ed30398f))
* **platform:** 🧵 isolate log and console writers per instance ([6a942ec](https://github.com/caunt/Void/commit/6a942ece1b3bfe21376319827b00e93377f9fca2))
* **player-extensions:** 🐛 allow serverbound packets to be registered without a link ([b5c8f11](https://github.com/caunt/Void/commit/b5c8f11a250a325eb37ef7b8422872e874146685))
* **player:** ✨ return early after direction detection ([77bca1e](https://github.com/caunt/Void/commit/77bca1ebf9bae89e1c9458925f07cf4183513881))
* **player:** 🐛 prevent unwanted player reconnection ([5cc3260](https://github.com/caunt/Void/commit/5cc3260e39c35e82725fc07aa5fb454cb463e981))
* **plugins:** 🐛 handle file URLs as local plugins ([3f3f290](https://github.com/caunt/Void/commit/3f3f290495f425e3289308d87c8f16be3e081340))
* **plugins:** 🐛 handle invalid plugin urls and paths ([2f32b45](https://github.com/caunt/Void/commit/2f32b45b3561899e0c7f90cf2d360597f1eb8ddc))
* **plugins:** 📝 correct NuGet resolver comment ([e94921c](https://github.com/caunt/Void/commit/e94921ce81a5971ee61bb5f17109b633cc924398))
* **plugins:** 🧹 ensure aggressive GC for unload ([32ccc4a](https://github.com/caunt/Void/commit/32ccc4a724a13f06a7a7617a09b298894a810393))
* **promptreader:** 🐛 remove redundant buffer update ([87cb233](https://github.com/caunt/Void/commit/87cb233c21182be34975b22e313878acd1afc90a))
* **properties:** 🐛 improve WorkingDirectory assignment ([da47429](https://github.com/caunt/Void/commit/da47429de52bd532b4d599ecde86a54c50e6ce58))
* **proxy:** ✨ clear only affected packet registries on phase changes ([6e5c57d](https://github.com/caunt/Void/commit/6e5c57d8124998f31fbbe51e774721b36d4b4f52))
* **registries:** 🐛 update player phase at last post order ([b42e61b](https://github.com/caunt/Void/commit/b42e61b75c8bf99b8ade671c330d954667aef762))
* **registry-service:** 🐛 clear registries early on phase change ([36e1814](https://github.com/caunt/Void/commit/36e18146b8b3095f3a0d4fea21a8e5ec87a1bf0d))
* **registry:** 🐛 clear registries only by current protocol support plugin ([8467dd5](https://github.com/caunt/Void/commit/8467dd56ea79c42aa8d5d162ba611ccff447f056))
* **registry:** 🐛 prevent enumeration of wrong registries for possible plugin packet codecs ([cf5163b](https://github.com/caunt/Void/commit/cf5163bcfb78b64fc91c50a0e83e12c9f8bfcdd8))
* **registry:** 🧹 clear less registries on phase changes ([2328c02](https://github.com/caunt/Void/commit/2328c02edc1a55b07d31ee3c9ef2ad815bf21446))
* **resolver:** 🐛 improved assembly matching and logging ([67aa38a](https://github.com/caunt/Void/commit/67aa38a305740333723bcb78aad6170a997d5263))
* **run-options:** ✨ add missing separator to working directory path ([6a03b69](https://github.com/caunt/Void/commit/6a03b6922cb9797aa2a0599c9b4958930addf314))
* **servers:** ✨ log registered servers on startup ([77b7b83](https://github.com/caunt/Void/commit/77b7b839ba9ff7dd6e9f65aea95052c11cfc5108))
* **tests:** ♻️ inherit environment variables in spawned processes ([4424c1b](https://github.com/caunt/Void/commit/4424c1b704e02ddbf3fa430ad5d657b8e101a20a))
* **tests:** 🐛 access build property in builds array ([4fb8738](https://github.com/caunt/Void/commit/4fb87384151c28e661b881050018cfd6a6969ed7))
* **tests:** 🐛 add contact URL to HttpClient User-Agent for PaperMC API ([9910a51](https://github.com/caunt/Void/commit/9910a51240db7f5dc4d01f6f308355ff1d19efea))
* **tests:** 🐛 add fallback for edge case with all pre-release versions ([24f6cc7](https://github.com/caunt/Void/commit/24f6cc7f56dc605f3b0be0db6fd043c49762fa77))
* **tests:** 🐛 add missing /builds to PaperMC API endpoint ([0945fc4](https://github.com/caunt/Void/commit/0945fc4a6fd60af9553f3f221f75da87bc572927))
* **tests:** 🐛 correct LINQ lambda expression for suffix filtering ([6b2540a](https://github.com/caunt/Void/commit/6b2540ae6ce1ffeaee8d20364a982e53b4747bb2))
* **tests:** 🐛 filter out pre-release versions and use correct API endpoint ([5b39211](https://github.com/caunt/Void/commit/5b392115d430db34d5ddbc033fef763be48981f9))
* **tests:** 🐛 update PaperMC API endpoint to include /versions path ([0c96ffa](https://github.com/caunt/Void/commit/0c96ffada627cfb29980c8cceb864429cb0f1335))
* **trace:** ✨ enhance event handling order ([e414cc5](https://github.com/caunt/Void/commit/e414cc563e9c6ec70d45b7f5b9e807234e635259))
* **transformations:** ✨ register transformations after respawn packet ([a60752b](https://github.com/caunt/Void/commit/a60752b070fba136bb85e583faba569fbaf981a7))
* **velocity:** ✨ fixed packet registration ([ee92bc7](https://github.com/caunt/Void/commit/ee92bc7864ff96191b3416446a9bd620ade63749))


### Performance Improvements

* **buffers:** ⚡ accept ReadOnlySpan&lt;char&gt; in WriteString ([1463242](https://github.com/caunt/Void/commit/1463242bca8a930aa5786adcf5b6c7809ee001a0))
* **commands:** ⚡️ avoid lower-case allocation in bool suggestions ([7e13124](https://github.com/caunt/Void/commit/7e131244d09fbed907e90ddc026b8f3c42ab1832))
* **commands:** ⚡ avoid lowercase allocation ([9be6bb2](https://github.com/caunt/Void/commit/9be6bb2311c48d6269a46a3fffb7d84d36eef33a))
* **common:** ♻️ use span-based stream write ([c0de8b5](https://github.com/caunt/Void/commit/c0de8b5543563de02480707ebe0819d1644d321d))
* **common:** ⚡ replace BitConverter with BinaryPrimitives ([6cd134d](https://github.com/caunt/Void/commit/6cd134dafd2ad9403912ef9338273cefb184ff34))
* **components:** ⚡ avoid params allocation in legacy serializer ([84965c8](https://github.com/caunt/Void/commit/84965c86d781ffaa474af4bc157cd2b3e74cdf02))
* **components:** ⚡ use span for hex color check ([e871e0e](https://github.com/caunt/Void/commit/e871e0ee43568568284c22429a2257d355f580d4))
* **forwarding-support:** ♻️ avoid split allocation ([326711c](https://github.com/caunt/Void/commit/326711c109eb8269676574dabb2aad7ca39365b2))
* **forwarding-support:** ⚡ eliminate string allocation ([a8fafe9](https://github.com/caunt/Void/commit/a8fafe95239324eeadbe4c41545f7d887f4b05a7))
* **forwarding:** ⚡️ cache maximum forwarding version ([904894c](https://github.com/caunt/Void/commit/904894c65b94bcb1b54d6a0b8f42c841bbd41e28))
* **minecraft:** ♻️ avoid StartsWith for color parsing ([4d4fae8](https://github.com/caunt/Void/commit/4d4fae8f8d94747394b94c2dc42e7da730af9076))
* **minecraft:** ♻️ avoid substring allocation in literal command node ([1d79daa](https://github.com/caunt/Void/commit/1d79daa78af176185c55345cc29278dfd7d2fbbc))
* **minecraft:** ♻️ use stackalloc for VarLongProperty ([50f1919](https://github.com/caunt/Void/commit/50f1919d74ebef912b3921abdefc27af9aa85c78))
* **minecraft:** ⚡ avoid array copy when creating BinaryProperty from stream ([f59fd0d](https://github.com/caunt/Void/commit/f59fd0d33362f10e42103e64fb59a948fe8e442f))
* **minecraft:** ⚡ avoid array copy when serializing NBT property ([cbf8b4b](https://github.com/caunt/Void/commit/cbf8b4beb2be51174a7745e553769fa464cd1108))
* **minecraft:** ⚡ avoid substring allocation in long parser ([53f1c73](https://github.com/caunt/Void/commit/53f1c733fa58e71c533d1218abdf6c37320d0758))
* **network:** ♻️ avoid extra allocation in StringProperty ([f5b6db2](https://github.com/caunt/Void/commit/f5b6db2458b1b7b1fe6bf03c22a916bd2a0ab0a6))
* **plugins:** ⚡ use span copy for IV rotation ([8535957](https://github.com/caunt/Void/commit/8535957431470ab4580b488226ef56a544a2e7a1))
* **tests:** ⚡ avoid params allocation in CollectingTextWriter ([0f304c1](https://github.com/caunt/Void/commit/0f304c1e90ab04698823c2fd1e46dc87bde608f2))

## [0.5.4](https://github.com/caunt/Void/compare/v0.5.3...v0.5.4) (2025-08-01)


### Features

* add cancellation support to entry point ([e68e37b](https://github.com/caunt/Void/commit/e68e37b350be559ba17b76b7de5e790fc7adecb8))
* add test project reference to solution ([9774e0b](https://github.com/caunt/Void/commit/9774e0bbc5a25ba60def3aca3d5ad63f6fa08397))
* add TryParse for Uuid ([caf19c6](https://github.com/caunt/Void/commit/caf19c6310f8510c72c51323cd9dd94c737c6d9c))
* allow specifying custom log writer ([7a77295](https://github.com/caunt/Void/commit/7a77295ce73f4a64a5bc130533db0e31554ac2fa))
* **auth:** ✨ add offline mode ([6c856fc](https://github.com/caunt/Void/commit/6c856fce5dec082c8360055d4112fd2b9de086a4))
* **docs:** expand SEO keywords ([8c07367](https://github.com/caunt/Void/commit/8c073672804d436133cbf1161e2f2d08354f189b))
* **docs:** improve SEO metadata ([3a4684f](https://github.com/caunt/Void/commit/3a4684f89ad753488e651ad69008c3545fdd9809))
* **encryption:** ✨ add support for offline mode ([c1afcd6](https://github.com/caunt/Void/commit/c1afcd68356d31fc856fe675bbdbfa5d7fada0ca))
* **platform:** ✨ add interface CLI option ([10746b5](https://github.com/caunt/Void/commit/10746b5711bfc3c850499ad13c0e2ea72f593661))
* **platform:** ✨ add logging level option ([032b3f8](https://github.com/caunt/Void/commit/032b3f8a6883e5be85b166996cc0fe491b8209a6))
* **platform:** ✨ add port CLI option ([2ff4089](https://github.com/caunt/Void/commit/2ff40896f93326d7a08fa0e90565d7f9e05408ce))
* **platform:** ✨ add server CLI option ([1901aff](https://github.com/caunt/Void/commit/1901aff9176ae0ab37f18995548d6cc01372d6f3))
* **platform:** ✨ name CLI servers sequentially ([b852e5e](https://github.com/caunt/Void/commit/b852e5ed6ed7869ea2a10daad963179545052868))
* **platform:** add ignore-file-servers option ([3aa41c2](https://github.com/caunt/Void/commit/3aa41c24ef91c42ae72ba48ae5c61ca8a1048dd7))
* **protocol:** ✨ add new protocol versions ([e1a76ec](https://github.com/caunt/Void/commit/e1a76ec9f22bbb296847eb25284a60eee4282e8a))
* **protocol:** ✨ improve string representation of names ([3bedd1a](https://github.com/caunt/Void/commit/3bedd1ab70eb432d50fbe36c5275583789c47e7c))
* **registry:** ✨ update mappings for new protocol versions ([9776ccb](https://github.com/caunt/Void/commit/9776ccb2a44e3bf22e395fc7048b079be310ae1b))
* **servers:** ✨ validate server arguments ([2690724](https://github.com/caunt/Void/commit/269072471796309eaebc11f3c0e17ae425a8b1d8))
* **tests:** ♻️ reset integration logs before each run ([563a2da](https://github.com/caunt/Void/commit/563a2da7c99bbd51b769cd6cb6904cdd78805f25))
* **tests:** use JRE 21 for Minecraft integration ([e165115](https://github.com/caunt/Void/commit/e1651159dc57d383999e32f930d70aefc02b280a))


### Bug Fixes

* add to RunAsync cancellation support ([db1fc77](https://github.com/caunt/Void/commit/db1fc7724be7e57b6a684762f5b8a8071f8457ac))
* **benchmarks:** 🐛 remove null-forgiving operator ([8dd5cb7](https://github.com/caunt/Void/commit/8dd5cb790efca1d00cf80d9c279adfdef8a8bf3c))
* **buffers:** 🐛 correct memory stream length comparison ([4a631b1](https://github.com/caunt/Void/commit/4a631b1d982581438dbafa07bc5a0341a2d838dd))
* **ci:** set GitHub token for test steps ([5d62062](https://github.com/caunt/Void/commit/5d620622239e9d037a383d51b1a0e5342c8f112a))
* **common-network:** 🐛 handle disposed stream ([4503d70](https://github.com/caunt/Void/commit/4503d70745c817e889bffb14522dfc999d719fd5))
* **common:** 🐛 correct invalid cast message ([bbe7748](https://github.com/caunt/Void/commit/bbe7748c90b885dc13ca2061254ddd1bc4fc1852))
* **common:** 🛠️ remove null-forgiving operator ([2ca845e](https://github.com/caunt/Void/commit/2ca845e67a45ae78f74af4ac53bcf9dc38b8bd02))
* **components:** ♻️ write nbt as unnamed after 1.21.5 ([aead5bf](https://github.com/caunt/Void/commit/aead5bf3d3784193e48c7203b9463e58578ef63a))
* correct error message formatting ([0d4a8eb](https://github.com/caunt/Void/commit/0d4a8eb0650e4681abdcd55f2b99501d6635dade))
* correct java binaries variable name ([ba487cd](https://github.com/caunt/Void/commit/ba487cd68ff5c0ebc33e8969c47a8cc4a3b00d98))
* correct typos in code ([1559b90](https://github.com/caunt/Void/commit/1559b90919abab1d6f9ab63be20602e54dd34de3))
* correct VarLong and UUID int array handling ([c4d6e83](https://github.com/caunt/Void/commit/c4d6e833587b48cca71b06cf18205f017779540f))
* **dependencies:** 🔧 update dependencies ([470135e](https://github.com/caunt/Void/commit/470135eb5c0f9feae9a3d860a83fb38fae4618e4))
* **docs:** 📝 separate distribution header ([06a1ff0](https://github.com/caunt/Void/commit/06a1ff02f3bf9789b17eded54b4c0e7687e7efe1))
* **docs:** 🔒 authenticate release fetch ([1fe57ab](https://github.com/caunt/Void/commit/1fe57ab1e48802c44131a5e8532cf107930d8748))
* **docs:** correct grammar in features guide ([c2253de](https://github.com/caunt/Void/commit/c2253de3b39b5f5a676438c071898e0664048774))
* encapsulate entry point logic in EntryPoint class ([850971c](https://github.com/caunt/Void/commit/850971c80c5ac25e61045bafefdcf978a6db5864))
* **errors:** 🐛 clarify packet registration error message ([0425a60](https://github.com/caunt/Void/commit/0425a60d46562bd9ff0814fb41d6206f370d50fc))
* **events:** 🐛 remove null forgiving operator ([9761b49](https://github.com/caunt/Void/commit/9761b49bdd91469b0fa97af8c43d59eb9c0ad2d4))
* improve console input/output handling ([61d2219](https://github.com/caunt/Void/commit/61d22190b821192a9f183f338517bef5e1162e58))
* **links:** 🐛 correct comment typo ([9e8e34f](https://github.com/caunt/Void/commit/9e8e34fcc66633d4f4cf435564c4dcc54452f94a))
* **logging:** 🔧 enhance connection log details ([661c9bf](https://github.com/caunt/Void/commit/661c9bfff5e81e002b0912d804f95f650b514ac8))
* **logging:** 🔧 improve logging level assignment ([2ad7cfb](https://github.com/caunt/Void/commit/2ad7cfb922b33331bab8db30e593a60ba4e94289))
* **minecraft:** 🐛 correct float array color precision ([4ba0d4a](https://github.com/caunt/Void/commit/4ba0d4a59fc14260523067811c25ef0491cf2c3c))
* **minecraft:** 🐛 correct grammar in slice exception message ([8b1f1d0](https://github.com/caunt/Void/commit/8b1f1d0dd7986dc30285aae9fb2cbc50bad230bb))
* **mojang:** 🔧 update offline status handling ([825ebca](https://github.com/caunt/Void/commit/825ebca25c1ebf878072acbe21bbfdafe8d00f77))
* **nuget:** 🐛 select newer versions correctly ([e7fc187](https://github.com/caunt/Void/commit/e7fc187a1c046211acfbbe46bd8041f9377f8292))
* **packets:** 📝 temporary disable SystemChatMessagePacket transformations ([e86db88](https://github.com/caunt/Void/commit/e86db88499afdd6f52e2a8ff09eb70a89faa4db7))
* **platform:** ♻️ reinitialize logger for each run ([47e2a0b](https://github.com/caunt/Void/commit/47e2a0bd1950dde5a92c8489f9b374928f587215))
* **platform:** ♻️ update offline mode configuration ([3acbd40](https://github.com/caunt/Void/commit/3acbd40885d723a8cc4b5bd5634e8a56a2e2d3bc))
* **platform:** 🔧 use offline mode CLI option value ([0fb47cf](https://github.com/caunt/Void/commit/0fb47cfde640cb2483a132aa6bb4abc0c03a783d))
* **platform:** 🔨 avoid null-forgiving operator ([8b1ba03](https://github.com/caunt/Void/commit/8b1ba033dd520aad79781d2f46c3352e64222217))
* **plugin:** 🐛 make linq callback async ([1bbbdfc](https://github.com/caunt/Void/commit/1bbbdfc88f79a2970ada027af3d5dbaa497012e4))
* **plugins:** 🐛 remove braces for single-line lock ([8eec7ef](https://github.com/caunt/Void/commit/8eec7efc0a011a13333c31eb7e34a44cba91878d))
* **plugins:** 🩹 remove null-forgiving operator usage ([fff0888](https://github.com/caunt/Void/commit/fff0888db4e2a0673dde284f3239911c4c7fa369))
* **plugins:** use injected HttpClient directly ([841d677](https://github.com/caunt/Void/commit/841d67789cd9a6569750381d776ae2cc4b40fb61))
* prioritize env var setting over in-file configuration ([15fa575](https://github.com/caunt/Void/commit/15fa5751154a287da2a0cb1a0448b6c2b0c99796))
* **protocol:** ✨ update string representation of versions ([41a4d71](https://github.com/caunt/Void/commit/41a4d71ca602017f73dde1007068e84916407591))
* **protocol:** 🔧 update signature data handling ([ac953bc](https://github.com/caunt/Void/commit/ac953bcca23b0c3ca51ba2b4365f4bfda75484a9))
* **protocolsupport:** remove null forgiving operator from login packet \U0001F4DD ([cc7135c](https://github.com/caunt/Void/commit/cc7135cf6ea91d96ebdee33a1ab018c037056c88))
* **registry:** 🐛 correct varlong property encoding ([111f1bf](https://github.com/caunt/Void/commit/111f1bf1636185994a70b309440ef1c05baa344c))
* **servers:** 🔧 fix --ignore-file-servers option usage ([9b5d814](https://github.com/caunt/Void/commit/9b5d81419eb4c95ea20f546b6067b20c02bdf3ea))
* **settings:** 🔧 update Offline property for runtime use ([07be838](https://github.com/caunt/Void/commit/07be838181a3abc3fbb47f3e3db4fc5a6d7c2c6b))
* simplify hash computation in GuidHelper ([1ea7f98](https://github.com/caunt/Void/commit/1ea7f98d591466f65e0597d005cf31a204c4f6e2))
* **terminal:** 🩹 remove unsupported constructor ([c81855c](https://github.com/caunt/Void/commit/c81855cb6d39359760b401e750f5c184b7355f4b))
* **tests:** ♻️ use interactive HeadlessMC ([4e446b1](https://github.com/caunt/Void/commit/4e446b18b5c371395941dea5dc5328c47db19099))
* **tests:** cancel process reads in PaperMC test ([1621686](https://github.com/caunt/Void/commit/16216865346e47b28a2bb0f09af30321fc69c508))
* **tests:** correct paper working directory variable ([3311808](https://github.com/caunt/Void/commit/3311808f97c562db0c4af95b4cfcd4e416ae8ca1))
* **tests:** import MITM certificate for portable JRE ([5790d36](https://github.com/caunt/Void/commit/5790d36bc78087b408442aa7c0b56fc87a501718))
* **transformers:** ✨ update hoverEvent processing to support Minecraft Console Client behavior ([304ce72](https://github.com/caunt/Void/commit/304ce72ce320b5b0fdb1c039328a21296b5d1b9f))
* wait readiness first ([9c6e52c](https://github.com/caunt/Void/commit/9c6e52cb753d0d1f746db83616e60be6d03fee31))


### Performance Improvements

* **buffers:** ♻️ use stackalloc for primitive writes ([b313cd3](https://github.com/caunt/Void/commit/b313cd37adadd61125abb875a58f398a9488ee40))

## [0.5.3](https://github.com/caunt/Void/compare/v0.5.2...v0.5.3) (2025-05-25)


### Features

* added manually requested stop reason to link events ([1fdbb07](https://github.com/caunt/Void/commit/1fdbb072efefb6c743c8e6c05c8d0cbf5775bc2f))
* added reason to link stop events ([04a5c64](https://github.com/caunt/Void/commit/04a5c646384768a87c1d6d00ebbd99099fd5b7f0))


### Bug Fixes

* proxy awaits for bind address to release ([7d7e54c](https://github.com/caunt/Void/commit/7d7e54cf649d2671b3ebc5c67902d02aadce0950))
* read & write text components nbt as unnamed after minecraft 1.20 ([c8c2495](https://github.com/caunt/Void/commit/c8c24954cc4a4b75479adab2271cf1b9bc1615af))

## [0.5.2](https://github.com/caunt/Void/compare/v0.5.1...v0.5.2) (2025-05-24)


### Bug Fixes

* changed player disconnection path ([1c901e9](https://github.com/caunt/Void/commit/1c901e97f02bb7a98167d68209421563a0c5ebd6))
* handle online-mode servers with authentication side set to proxy ([3b38c96](https://github.com/caunt/Void/commit/3b38c96765f18d9d6e5405f3c42b195616af2247))
* nbt compound and list parsing are now in one method ([f9d524b](https://github.com/caunt/Void/commit/f9d524b7abc0e3738738843aa749594d0858d9d2))

## [0.5.1](https://github.com/caunt/Void/compare/v0.5.0...v0.5.1) (2025-05-16)


### Bug Fixes

* bump compiled release version ([493b174](https://github.com/caunt/Void/commit/493b174e2d991e554caacfc3af19385aae3ae4dd))

## [0.5.0](https://github.com/caunt/Void/compare/v0.4.0...v0.5.0) (2025-05-15)


### ⚠ BREAKING CHANGES

* update environment variables names

### Features

* added many watchdog api methods ([9670d18](https://github.com/caunt/Void/commit/9670d1821f0481205f056eb645b6eea236c32837))
* added VOID_WATCHDOG_ENABLE environment variable ([5bddb90](https://github.com/caunt/Void/commit/5bddb9018a0a7459c4ea5fbff85bea5ee37aa7f1))
* added watchdog simple web application ([b82da8a](https://github.com/caunt/Void/commit/b82da8ad5fb09e918ccf0a568284ad600950f77c))
* allow pausing accepting connections ([dc772e8](https://github.com/caunt/Void/commit/dc772e861e4057d39eabbf225249eb277f942eba))
* allow specifying address to bind watchdog ([2790e7a](https://github.com/caunt/Void/commit/2790e7af73708f8065e118187b088f91a9ef3095))
* allow specifying repositories with VOID_NUGET_REPOSITORIES environment variable ([627e243](https://github.com/caunt/Void/commit/627e2435d546d72d163e607879ec5f6f2315666d))
* allow specifying whether to kick players on proxy shutdown ([704e89b](https://github.com/caunt/Void/commit/704e89b7d69a522a704589a95e4beae1b3d358c4))
* discover servers automatically ([d700d6f](https://github.com/caunt/Void/commit/d700d6f8beae7287d6640f0b420219324e5bba01))
* include asp.net runtime assemblies into a build ([860ddc3](https://github.com/caunt/Void/commit/860ddc347c5205b9cc7d5102f42086ef1101a5fb))
* introduce watchdog plugin ([0fb4554](https://github.com/caunt/Void/commit/0fb4554db98de860500737a17f923168b09d7f4b))


### Bug Fixes

* added proxy statuses ([54a45ab](https://github.com/caunt/Void/commit/54a45abc3a820c9ed83e6fae3e182efc1a74d92d))
* added version property to all builds ([e690e1e](https://github.com/caunt/Void/commit/e690e1eac5c051b154972490869a4b0dcc6f932b))
* allow semicolon escaping ([fe2e1d4](https://github.com/caunt/Void/commit/fe2e1d421e41ccbe7b0d684390d7a7fee7f02250))
* allow terminals without tty ([7be8071](https://github.com/caunt/Void/commit/7be8071555474c0243e12b2fe1cf4fd9a442bc1e))
* cache toml mapped assembly types ([9b6c529](https://github.com/caunt/Void/commit/9b6c52974439cf7af5b2b1382e374cf6c108e1ee))
* conditional dotnet sdk ([70444ed](https://github.com/caunt/Void/commit/70444edcdf5fd4bf3a40cbfae5a3eb3c5b476b4b))
* conditional remove aspnet sdk ([c5a922a](https://github.com/caunt/Void/commit/c5a922ad93b901fa28a61fff41332024a216858e))
* configure hosting shutdown timeout to infinite ([b35ada2](https://github.com/caunt/Void/commit/b35ada2d95bb62e5f9dfad3e85727e4b14e2459c))
* custom conditional variable for bionic sdk switch ([7181430](https://github.com/caunt/Void/commit/71814300084261910e4802a2b288f7f3e5e0ff6d))
* dispose cancellation token source after event is handled ([d815c5d](https://github.com/caunt/Void/commit/d815c5d93772380e04131e8235e272360dddf4c6))
* do not include runtime identifier in solution build ([66255ad](https://github.com/caunt/Void/commit/66255add92df661f63e210b8f0f4d8958fac9ce1))
* do not read console when input is redirected ([3fd46b2](https://github.com/caunt/Void/commit/3fd46b27a4ca12f00fb9590116f1b21794fc0121))
* do not specify list of rids ([1af547c](https://github.com/caunt/Void/commit/1af547c62a4480ad469c77703a2e098246f9a3af))
* exclude watchdog plugin from bionic builds ([64cd99b](https://github.com/caunt/Void/commit/64cd99bc849fde7261d2705a0d1c5c362f37944f))
* include aspnet framework when not bionic ([75238e6](https://github.com/caunt/Void/commit/75238e62b3e869b95013a797464ac1c003d32b71))
* include rids via msbuild property argument ([0f7e6d6](https://github.com/caunt/Void/commit/0f7e6d6a0fcc2a15ed3777d97018721f0107d0c0))
* include sdk parameter in build ([90ed3bb](https://github.com/caunt/Void/commit/90ed3bb691a89a707ca0b4c01dffe87bcb0eca89))
* loading shared assemblies ([ab36bb0](https://github.com/caunt/Void/commit/ab36bb0679a9ea71c02f32f7672af722357e0653))
* print proxy version at start ([e82513b](https://github.com/caunt/Void/commit/e82513b6722f644725baabf0f016546ad81f26eb))
* pull assembly version from attribute ([4cd32ff](https://github.com/caunt/Void/commit/4cd32ff2460026e761aa0dec35bde23cf8be62c5))
* register proxy as hosted service ([89541b8](https://github.com/caunt/Void/commit/89541b80f175c23b775adf2025084682e96b29a7))
* rename manifest_version to fallback_version ([5a31032](https://github.com/caunt/Void/commit/5a31032c294b3b74c029ca6a392370f743ce1e06))
* update proxy interface to support manual stopping ([1ba2401](https://github.com/caunt/Void/commit/1ba2401ea8d1d526c45e7f031890a7ed6a904582))
* use latest release v tag in unstable builds ([2b96f58](https://github.com/caunt/Void/commit/2b96f585a4849e2a36d003ed4f0a297f3d4d9b1a))
* waiting for players to disconnect before emitting application lifetime stop ([b08b0ce](https://github.com/caunt/Void/commit/b08b0cefa1fdc892b8f54a88e15dfb2ae3055cbc))
* watchdog is not started anymore if `Enabled` settings is set to false ([5bddb90](https://github.com/caunt/Void/commit/5bddb9018a0a7459c4ea5fbff85bea5ee37aa7f1))


### Code Refactoring

* update environment variables names ([5bddb90](https://github.com/caunt/Void/commit/5bddb9018a0a7459c4ea5fbff85bea5ee37aa7f1))

## [0.4.0](https://github.com/caunt/Void/compare/v0.3.4...v0.4.0) (2025-05-08)


### ⚠ BREAKING CHANGES

* dependency resolvers interface

### Features

* include nuget repositories from command line options ([a8421bc](https://github.com/caunt/Void/commit/a8421bc9de8cedec693f70845f86d85831e036a6))
* parameters injection in service instantiation ([a8f777a](https://github.com/caunt/Void/commit/a8f777a469abbddaf5230a3400233757cb362bb3))
* register custom nuget repositories option ([e3ba723](https://github.com/caunt/Void/commit/e3ba723417a3cb6c0faf135fff2c9c731a4f3127))


### Bug Fixes

* allow adding repositories from plugins ([75a2159](https://github.com/caunt/Void/commit/75a215985e9fd55d29832140a561af3e3da66a0d))
* allow loading directories with plugins ([b81e0ab](https://github.com/caunt/Void/commit/b81e0ab7c23fb21de0e2ec873ee4e90a13600640))
* collection modified ([4d55cae](https://github.com/caunt/Void/commit/4d55caec72be9813254cbdc1ea401626cbf627da))
* conditional else ([165f88e](https://github.com/caunt/Void/commit/165f88e432d0aa1301c1476003c6d79f478e88ba))
* configure commandline with default options ([763274d](https://github.com/caunt/Void/commit/763274d8b81171c55238be5e889149b2c2afe7a3))
* custom repositories nuget authentication ([1f36778](https://github.com/caunt/Void/commit/1f36778268a78587751fe5c11c7157a5f55ddfb9))
* do not create services instances manually ([f2234b1](https://github.com/caunt/Void/commit/f2234b183222d83a8693a6c613079d62c7e1e4ee))
* enable more default commandline options ([71e44f5](https://github.com/caunt/Void/commit/71e44f5b93fa920c9a812047308bf82381aa0c60))
* register options via commandline ([94b425e](https://github.com/caunt/Void/commit/94b425e9038d745efc85438751b191e0c2636be5))
* update plugins option name ([7673b4e](https://github.com/caunt/Void/commit/7673b4ec6aeea5764363364ef3d3804d5758ffe0))


### Code Refactoring

* dependency resolvers interface ([1eec2b9](https://github.com/caunt/Void/commit/1eec2b9d0300cfaf2e8bfdc2e97676e51c4571d2))

## [0.3.4](https://github.com/caunt/Void/compare/v0.3.3...v0.3.4) (2025-05-06)


### Bug Fixes

* add plugins layer to player context composite ([5600129](https://github.com/caunt/Void/commit/560012989dee5ee7f24150945af798f53c003b65))
* allow empty components deserializing ([0d0b255](https://github.com/caunt/Void/commit/0d0b2556a1f9ec08cb0079e878d1907856bde7bc))
* apply convert attribute on uuid ([dac32a2](https://github.com/caunt/Void/commit/dac32a2fb0cfce027a4d6fa13d54bcfd94399940))
* do not update configuration files from disk if they were just saved ([54e01c5](https://github.com/caunt/Void/commit/54e01c53b72b9e44d35b68c24986e44282618639))
* mojang service authentication ([236a403](https://github.com/caunt/Void/commit/236a40350118e5bc7c9fa8b2262581005a6c90a1))
* read and write as property names for uuid converter ([74b5854](https://github.com/caunt/Void/commit/74b585452b8ad36a98b151ede7ede07cceb6385c))
* replace updates skips with half second cooldown ([be9a358](https://github.com/caunt/Void/commit/be9a3580114e235a245de97799e6a773ae8de949))
* synchronize configuration loading ([b12b128](https://github.com/caunt/Void/commit/b12b128a27e90f00f9a1cf5a57c66efcd96d8da8))
* synchronize configuration service even more ([a131819](https://github.com/caunt/Void/commit/a1318195bee43095dd9f899c900710d1a518c2cb))

## [0.3.3](https://github.com/caunt/Void/compare/v0.3.2...v0.3.3) (2025-05-04)


### Bug Fixes

* docker image tags single quotes ([e922c85](https://github.com/caunt/Void/commit/e922c85eb4f8dccd27fcc1875eaefc1c947b2ad7))

## [0.3.2](https://github.com/caunt/Void/compare/v0.3.1...v0.3.2) (2025-05-04)


### Features

* configurable velocity forwarding ([1a64fb3](https://github.com/caunt/Void/commit/1a64fb381c487babf13e2734b7975635085a472c))


### Bug Fixes

* build configuration cache from existing files ([7dec9d7](https://github.com/caunt/Void/commit/7dec9d794ccf7ce30b96a99b2e99269f2c9a7226))
* ensure player is disposed after kick ([f70cbbc](https://github.com/caunt/Void/commit/f70cbbc72b37d7d504777f6e72ad88b834512170))
* make velocity forwarding packets be decoded on plugin side ([b741357](https://github.com/caunt/Void/commit/b74135739df8ea642f4df45850c885f15da6fa4c))
* register plugin container before loading event ([55473e3](https://github.com/caunt/Void/commit/55473e32c9ccfef1e8a944074bbd18f2ea8808d1))
* send feedback about unloaded container ([276aafc](https://github.com/caunt/Void/commit/276aafc7b2e7badd8ab53002a8e76c1475be731d))
* velocity forwarding 1.19.* versions ([2cb6a9c](https://github.com/caunt/Void/commit/2cb6a9c063e030ae0ee2e77bbd4c4d90442c25b4))
* velocity forwarding enabled check ([0330b99](https://github.com/caunt/Void/commit/0330b99721ad81c5435b7060fbb9e0718d506b89))
* write string length in span buffers ([90be225](https://github.com/caunt/Void/commit/90be225da7b3c26278743ed8f123d6c3c80d725d))

## [0.3.1](https://github.com/caunt/Void/compare/v0.3.0...v0.3.1) (2025-05-04)


### Bug Fixes

* allow passing cancellation token to event listeners ([c89dfc1](https://github.com/caunt/Void/commit/c89dfc1f44127cb9ab91c0ef68adae7fbffbc2d3))
* allow registration event listeners without cancellation token ([fe1a4ff](https://github.com/caunt/Void/commit/fe1a4ffef54a86cc357e240df29da66652845575))
* dispose minecraft player context ([9dd962c](https://github.com/caunt/Void/commit/9dd962c0aad78628798e5068de308aff2f359c81))
* do not kick player twice ([eb1dddc](https://github.com/caunt/Void/commit/eb1dddce34be240638670c349230d92cd64c9f6f))
* example plugin commands registration ([b26a5cd](https://github.com/caunt/Void/commit/b26a5cdc4834910f45c2b7703d404a3fc987c49b))
* isolate runtime assembly resolver ([4969583](https://github.com/caunt/Void/commit/49695830bda47f2bc0cead299809522460b136af))
* make dependencies cancellable ([883342f](https://github.com/caunt/Void/commit/883342f06fa776d289fd95313f21af6e71da3b4d))
* move brigadier extension types to same namespace ([b26a5cd](https://github.com/caunt/Void/commit/b26a5cdc4834910f45c2b7703d404a3fc987c49b))
* override console name ([4daa833](https://github.com/caunt/Void/commit/4daa833ef1b8023f9c29b624f41a3797b7b3ae0a))

## [0.3.0](https://github.com/caunt/Void/compare/v0.2.3...v0.3.0) (2025-05-03)


### ⚠ BREAKING CHANGES

* remove IMinecraftPlayer interface, move all minecraft extension methods to IPlayer directly

### Features

* gh pages documentation setup ([b6fb947](https://github.com/caunt/Void/commit/b6fb94753bdfe93956629c8d601e86ca9c179726))
* setup docs pages ([2aaded2](https://github.com/caunt/Void/commit/2aaded23555be7a68f1d121b514d5e306c015af4))


### Bug Fixes

* gh pages permissions ([5ab9d61](https://github.com/caunt/Void/commit/5ab9d61599df9002dca39a09603f06b6e6aee38a))
* gh pages upload path ([ad6d638](https://github.com/caunt/Void/commit/ad6d638c088e2c03b1119036dc6d04ede09df6a0))
* msdi documentation link ([1e1bf3b](https://github.com/caunt/Void/commit/1e1bf3b96b6be6786a15d5936c1efe11a7ef5157))
* temporary rollback C# 14 extension type ([680a580](https://github.com/caunt/Void/commit/680a580486d19b9548bc347f47d24ae3b6c266fa))


### Code Refactoring

* remove IMinecraftPlayer interface, move all minecraft extension methods to IPlayer directly ([a240a1d](https://github.com/caunt/Void/commit/a240a1d24757750e9b7af5750d0565dc5d69d6a6))

## [0.2.3](https://github.com/caunt/Void/compare/v0.2.2...v0.2.3) (2025-04-28)


### Bug Fixes

* cleanup plugin devkit ([9883c29](https://github.com/caunt/Void/commit/9883c292a4e3cd2b831707955667b88fd5fc776d))
* more crossplatform plugin devkit ([eec8107](https://github.com/caunt/Void/commit/eec81071ee006f1f36b1f0e7d331ed55385fdbb0))

## [0.2.2](https://github.com/caunt/Void/compare/v0.2.1...v0.2.2) (2025-04-28)


### Features

* include plugin development kit in release ([4ac5930](https://github.com/caunt/Void/commit/4ac5930791c0d1904267bf7a6399171b4d5af06a))


### Bug Fixes

* plugin devkit properties evaluation ([bd07714](https://github.com/caunt/Void/commit/bd077141fdbbc2d5aa9d3cedd88fb94c5de86349))
* remove launchSettings.json from plugin devkit ([aebe396](https://github.com/caunt/Void/commit/aebe396e112cc5ed6e3124601b4e1cc1f4d7c9e1))

## [0.2.1](https://github.com/caunt/Void/compare/v0.2.0...v0.2.1) (2025-04-27)


### Features

* loading environment plugins ([97e300a](https://github.com/caunt/Void/commit/97e300acb98587c60497987caaf81d3205b040c4))
* plugin devkit ([20382bf](https://github.com/caunt/Void/commit/20382bf8d24c33fe60bf01d7ee344ca96f23b132))
* switch example plugin dependencies to nuget source ([5eaa7dd](https://github.com/caunt/Void/commit/5eaa7dd0f4d8e080dfbd8b342c852d66e6475cc3))


### Bug Fixes

* concurrent configurations map ([e93e995](https://github.com/caunt/Void/commit/e93e9950fb7a3addc18f1e6097e6208914f98b49))
* print reflection type load exceptions ([a5a8624](https://github.com/caunt/Void/commit/a5a86245bd51a87d0393741994c8e9ccfc00b254))

## [0.2.0](https://github.com/caunt/Void/compare/v0.1.0...v0.2.0) (2025-04-26)


### Features

* add clear plugin methods to registries ([d641d10](https://github.com/caunt/Void/commit/d641d10a98b8c863434363a2926e9506122e2bce))
* add IsDisposed property to IPlayerContext ([2555af7](https://github.com/caunt/Void/commit/2555af72e8a1e6237a93a3d99d1660dcbcbbf5b5))
* add PAPER_CONFIG_REPO environment variable for configuration ([7734d2a](https://github.com/caunt/Void/commit/7734d2a73f19a3252957f691d5084745b7d28602))
* add runtime identifiers to all projects ([4a41bcc](https://github.com/caunt/Void/commit/4a41bcc9e2f4ee83ffb8f299e537c218105f556e))
* add Source property to ListeningServiceProvider ([eb27de7](https://github.com/caunt/Void/commit/eb27de708c16e799efe180d15765feb98621ff10))
* allow injecting non-generic ILogger ([9fc8fba](https://github.com/caunt/Void/commit/9fc8fbaf5e499196a004f2d54a3dcafc70ff33d9))
* allow injecting player context in scoped services ([e0a1fc9](https://github.com/caunt/Void/commit/e0a1fc9b551be0d1fe965290630226cba8b362e6))
* composite services for players ([c603085](https://github.com/caunt/Void/commit/c603085f4b0252cc09424109e5da25addccf24cd))
* create per-player scopes in dependency service ([3a5fb1e](https://github.com/caunt/Void/commit/3a5fb1efe5d2536d1a906651e754f9ce1f138a66))
* detection if player is a minecraft player, upgrading its implementation ([569c5ef](https://github.com/caunt/Void/commit/569c5ef710c83ebd812588587d7330f9e01444e7))
* dry ioc container tracking ([0d68799](https://github.com/caunt/Void/commit/0d68799fc69ad2650b61bbc96751d742c3306059))
* enable preview language features ([4ee7183](https://github.com/caunt/Void/commit/4ee7183e3d74c3962f7beb91cee1181dabc8fbd3))
* enhance DependencyService for better service management ([0e9a6b2](https://github.com/caunt/Void/commit/0e9a6b270acbc30e66aa50b9103aecda6ccb9770))
* enhance scoped service registration in DependencyService ([77ac2eb](https://github.com/caunt/Void/commit/77ac2ebdb664a622ee704a6e087d78d81fc5622e))
* enhance service registration in HostingExtensions ([cbac74a](https://github.com/caunt/Void/commit/cbac74a6beb310ddcf8c36f5b6546133d1b391ae))
* extension methods for synchronizing collections ([f2a1493](https://github.com/caunt/Void/commit/f2a14932752db501fb92fd1f2f44c7a9426d7cc5))
* filtering and bypassing the filter of scoped player events ([1a578e0](https://github.com/caunt/Void/commit/1a578e0ce44d6f19c6cc2afa129d7d4051b1050c))
* player-scoped events for scoped services ([f2f681b](https://github.com/caunt/Void/commit/f2f681b18a41bd618a8850dfa9aa50fe98be570a))
* stable hash codes for IPlayer ([32ec5ad](https://github.com/caunt/Void/commit/32ec5adb47841baeb2ad893c5793ce072c5cf4eb))
* stable hash codes for players ([68f6e32](https://github.com/caunt/Void/commit/68f6e32a5b2a7550e332309ba063cdbe3f08b5b1))
* update packet/message handling in extensions ([55f8ebf](https://github.com/caunt/Void/commit/55f8ebfbe3b22268991a805723912de8d5156abf))


### Bug Fixes

* add thread safety to MinecraftPacketIdPluginsRegistry ([f26c85b](https://github.com/caunt/Void/commit/f26c85bfe7da27145ba94fb912c1d9acd64d47cd))
* add using directive for extensions in EntryPoint ([0ab5b28](https://github.com/caunt/Void/commit/0ab5b289f72349485245078620d58fa8b5345f2a))
* allow creating composite without preferred container ([2bb05d7](https://github.com/caunt/Void/commit/2bb05d7c98d90c2d7d41b9531a589724d450a411))
* allow non-scoped events triggering scoped listeners ([da5ea30](https://github.com/caunt/Void/commit/da5ea305dfe144b660a67be35025a1ceb0008497))
* always register plugin services outside root container ([55d99c5](https://github.com/caunt/Void/commit/55d99c51cb7d1dc9ee5e9bdbea275543fd7b6e43))
* avoid collection modified exceptions in plugin transformers ([97d2430](https://github.com/caunt/Void/commit/97d243014ac25c1b2ad090ddb9c43d17da323948))
* clear plugin registries later ([5391e8e](https://github.com/caunt/Void/commit/5391e8e725e23270d44334f1b2ea030e9db90d25))
* composite scoped service providers for players ([ce35f90](https://github.com/caunt/Void/commit/ce35f90858668ecc400d157ac02c94bc9eebf30a))
* consolidate player context disposal and clean up code ([b6199e2](https://github.com/caunt/Void/commit/b6199e2b5508764602f1c7c0b4057d5e5d1d38d7))
* correctly dispose scoped player composite and do not dispose player instantly ([a4a7719](https://github.com/caunt/Void/commit/a4a7719d066bfd81e824bbb33d848200ee7664e9))
* dispose player services later ([ff9b2fa](https://github.com/caunt/Void/commit/ff9b2fad49d1481a97ad0aa0640fc663876e1137))
* double ioc container tracking ([81ffa92](https://github.com/caunt/Void/commit/81ffa92f187319a8011436c8e63cd902f459d673))
* escalate container user permissions ([de971ea](https://github.com/caunt/Void/commit/de971ea32d78e373a800bf88c736dc32b929610a))
* explicitly register services as listeners ([e63f81d](https://github.com/caunt/Void/commit/e63f81d60b5b20becc06a3b16bbb7004a98cb610))
* improve composite layers in player scoped resolutions ([bc2291d](https://github.com/caunt/Void/commit/bc2291d724140f7341f10366bfc27afcd5316bdc))
* improve error handling in UnloadContainerAsync ([81f28f1](https://github.com/caunt/Void/commit/81f28f15f36bc21a35d34656f83e2f03ac396f8b))
* improve event filtering and service registration logic ([b91ba76](https://github.com/caunt/Void/commit/b91ba767364b5ef4252b138b04e2cbc0d9409065))
* indexing typo in unloading configurations ([4b4bbc4](https://github.com/caunt/Void/commit/4b4bbc4a8b6520eabacf60cf9aeb3891781443d0))
* let other listeners process disconnected player before disposing him ([0d1a354](https://github.com/caunt/Void/commit/0d1a35450e3514401eea0865982dfe722e31e167))
* pass weak references to event listeners pipeline ([a38a5dc](https://github.com/caunt/Void/commit/a38a5dc745d57c4ec53e0db5acf0d1699b13d316))
* register services as event listeners on resolve ([0c7522d](https://github.com/caunt/Void/commit/0c7522d9b751f5e4575f2cc1ff92c8f8d5120cd4))
* remove cache clearing after service unregistration ([7c5f1b4](https://github.com/caunt/Void/commit/7c5f1b4ed64cb702f6157a7a67ec5a0aeb3d1b76))
* remove recursion from rethrowing events for new listeners ([a4dbb58](https://github.com/caunt/Void/commit/a4dbb58a54e72c2a0263f7b0791accd2325101cd))
* reset binary message stream position after packet id var int ([b83b15b](https://github.com/caunt/Void/commit/b83b15b4bb3ebf500a4a193431e3de2623c51ea8))
* resolve scoped services via composite ([f9ad174](https://github.com/caunt/Void/commit/f9ad1741563dc5a2c25a019c7847b3c948ca8a1b))
* save forwarded properties from previous player implementation to bypass ObjectDisposedException when trying to access RemoteEndPoint ([9aa0796](https://github.com/caunt/Void/commit/9aa0796e5f03039e3fca4dc0a91d1a2f47e23ffb))
* subscribe as event listeners services resolved from dependency service ([dbb91e6](https://github.com/caunt/Void/commit/dbb91e6141464160d25c81c39c8b29f2d7916974))
* unloading plugins with per-player scoped services ([740cd18](https://github.com/caunt/Void/commit/740cd18c4a8842651df3a38b7a1f669f07ab3f25))
* update dotnet.yml to use .NET 10 SDK preview ([67c6ef7](https://github.com/caunt/Void/commit/67c6ef7c4cb3ba984b5304587d430bd7fc4f8b15))
* update player in player context ([aa9b732](https://github.com/caunt/Void/commit/aa9b732204d2dd4d2bfda910fabdaa3fd91432f0))


### Performance Improvements

* optimize BlockSize in RecyclableMemoryStreamManager ([5a831f3](https://github.com/caunt/Void/commit/5a831f3f2c8d7f55075304d5dc254d8f4a449b0c))
* optimize byte array allocation in forwarding plugin ([32f64f5](https://github.com/caunt/Void/commit/32f64f507b9f3237fbcae8baccd2e5881a840ca4))

## [0.1.0](https://github.com/caunt/Void/compare/v0.0.2...v0.1.0) (2025-04-21)


### Features

* adapt settings to configurations service ([efce7a5](https://github.com/caunt/Void/commit/efce7a57d69d8273bfb15e8ad3c70c9bbb301f6f))
* add chat command event handling ([94b81b3](https://github.com/caunt/Void/commit/94b81b34254aa3b55a4b4d3ccea7ba83e5041586))
* add chat message for item slot change ([60a17ee](https://github.com/caunt/Void/commit/60a17ee8ff05c71150b03418f91779daf91efccd))
* add command registration functionality ([40e049b](https://github.com/caunt/Void/commit/40e049b5c0dd6e94cc46b9ce3ca3364647addc6e))
* add conditional logging levels based on build config ([2925285](https://github.com/caunt/Void/commit/2925285b0460b47976f00684198edaa6f8a9420c))
* add Containers property to IPluginService ([c082867](https://github.com/caunt/Void/commit/c0828671b747f3aa2ceae5aaecd5b2d704f2c1de))
* add dependency management service ([1489e09](https://github.com/caunt/Void/commit/1489e09c7d4b0dc2aa2dadc2a54afb9a175a0955))
* add embedded debug information to project file ([23a22b3](https://github.com/caunt/Void/commit/23a22b394e4744e71834e8d510fdbbe56eb22981))
* add experimental channel for newer Minecraft versions ([56f6adc](https://github.com/caunt/Void/commit/56f6adc4f98b0f9fdebcfc16f444460dc30a196b))
* add GetServer method to PlayerExtensions ([0066cf2](https://github.com/caunt/Void/commit/0066cf2eca43ebd23e868d1b181d93d78dd778b1))
* add handling for created entries in event service ([ad1cac4](https://github.com/caunt/Void/commit/ad1cac455c0cae2bda06e30178bfc88fabe13809))
* add IApiPlugin interface to plugins namespace ([0d10d67](https://github.com/caunt/Void/commit/0d10d6710f3f85135aa5ed2bcd79d2c9340f97b1))
* add ICommandDispatcher and ICommandNode interfaces ([30881da](https://github.com/caunt/Void/commit/30881da1e9c57ca30824f06c0d7b0d4957120473))
* add IDependencyService and refactor dependencies ([edb9362](https://github.com/caunt/Void/commit/edb9362ec395f26820b39e5cc1105b9545147465))
* add intercepting service provider classes ([1beed48](https://github.com/caunt/Void/commit/1beed480aaf315a89f3379f929b4f916a8626bc8))
* add logging and player retrieval enhancements ([da02773](https://github.com/caunt/Void/commit/da02773e25bc6c46b316f4c81b3d87b282a5c7e4))
* add logging for event listener registration ([c775de0](https://github.com/caunt/Void/commit/c775de0526d9edb49efd55370b545fd450c9cd67))
* add logging to command services ([7d6869e](https://github.com/caunt/Void/commit/7d6869e96b6c5777bc6446c031981bb9510121b9))
* add logging to DependencyService constructor ([77777bc](https://github.com/caunt/Void/commit/77777bcec6ca36cef4b62dc35cefef5621bfed62))
* add NewUuid method ([7cffe35](https://github.com/caunt/Void/commit/7cffe3583a3cc1206f13b7e3c3bf0c580535afef))
* add recursive method to traverse command nodes ([ef40beb](https://github.com/caunt/Void/commit/ef40beb29a71ee556c8f14e21307e568985d225e))
* add reflection extension methods and namespace ([a0886e5](https://github.com/caunt/Void/commit/a0886e592ae128d396854a8c5b08763e1f2a100c))
* add RegisterListeners method to HostingExtensions ([5aa82b7](https://github.com/caunt/Void/commit/5aa82b74e436cb499ddc3e0dafdbdcac92edfc1d))
* add RootConfigurationAttribute and improve file naming ([b02a079](https://github.com/caunt/Void/commit/b02a0793d5fe34fab54ac1ec3734690ae2ddf1b5))
* add service descriptor support methods ([a4677d9](https://github.com/caunt/Void/commit/a4677d953f94d8a988d9150e9c8044886f7a1442))
* add SetPropertyValue method ([d8adc49](https://github.com/caunt/Void/commit/d8adc49e34afd99fc4eb3821b9fdb33d01573e10))
* **chat:** enhance chat service with settings loading ([64aff83](https://github.com/caunt/Void/commit/64aff839959a80ddc494084a9ad111fbc081c93d))
* clear brigadier nodes from unloading plugins ([55cf9e0](https://github.com/caunt/Void/commit/55cf9e0ed88c70468e4b59ddd7f90829185c00b2))
* **dependency-service:** enhance IDependencyService interface ([1174011](https://github.com/caunt/Void/commit/1174011eabed924582c932c6d42aec6ce0368217))
* **dependency-service:** enhance service management functionality ([cb4bdd3](https://github.com/caunt/Void/commit/cb4bdd3f9c8c5deea611f3b4b7ff6608ac93b44d))
* **dependency-service:** enhance service retrieval methods ([f90e8e9](https://github.com/caunt/Void/commit/f90e8e90cb8cf063c2b0240f8e291683135c75e4))
* enhance chat command handling and player interactions ([a1877b8](https://github.com/caunt/Void/commit/a1877b8d90244403c2a97c81ef641307d42a649e))
* enhance chat event records with origin parameter ([7dfcf2a](https://github.com/caunt/Void/commit/7dfcf2aed4b07229ee9891f84bc0d5e09fd3485d))
* enhance command execution handling ([0858ec1](https://github.com/caunt/Void/commit/0858ec1c0de0815a3ddc780235f89caf1ce9533e))
* enhance command handling in services ([1453e3f](https://github.com/caunt/Void/commit/1453e3f2099189dcd0f5bc89f21087b58a3e39de))
* enhance command handling with interfaces ([48e5e2a](https://github.com/caunt/Void/commit/48e5e2a102c90ac42a44e3b972a9ac3e069376b2))
* enhance command message handling ([952d940](https://github.com/caunt/Void/commit/952d940bf06625d3bed04f4cb68b9b4f59a6a698))
* enhance command processing in CommandService ([63ef7d2](https://github.com/caunt/Void/commit/63ef7d21229645782b3a61caa04d708335351e6f))
* enhance ConfigurationTomlSerializer functionality ([3b3c2f7](https://github.com/caunt/Void/commit/3b3c2f74b483eec4a5ac05c66f677578332fcae4))
* enhance dependency injection and service registration ([9505414](https://github.com/caunt/Void/commit/9505414ed4102f525116ce04dadd4cd3938f2615))
* enhance dependency management in service classes ([143047c](https://github.com/caunt/Void/commit/143047c19a65f62bcd59f4206ff305d890a8e862))
* enhance dependency service instance creation ([d6aa321](https://github.com/caunt/Void/commit/d6aa321cb7e46ee78fb753d04fe3cbe9199eaf5a))
* enhance event handling in EventService ([7c91d6d](https://github.com/caunt/Void/commit/7c91d6dd047f9de9932a3a0859d87e766ae38a2c))
* enhance IDependencyService with activation control ([370e102](https://github.com/caunt/Void/commit/370e102d122f0e9d7c3819bcd8293d6deda5a97d))
* enhance IEventService with new methods and properties ([b17d758](https://github.com/caunt/Void/commit/b17d758d8852f6e7edf4cf68c0ee00c20402b4df))
* enhance InventoryService with command registration ([411a7f9](https://github.com/caunt/Void/commit/411a7f97a1dd2bb30d4863bf0dbfb9c6604ef1f6))
* enhance IPlayer interface with command sourcing ([3dad012](https://github.com/caunt/Void/commit/3dad0126cadcdfc6639670414561a657a6a44f71))
* enhance Link and LinkService with cancellation support ([c923171](https://github.com/caunt/Void/commit/c9231718e6923a9fa0b418495b084b086c336b50))
* enhance logging and service configuration ([1e499cb](https://github.com/caunt/Void/commit/1e499cb9642a1a21c1cacadf929555b72691ab80))
* enhance moderation commands in ModerationService ([5538910](https://github.com/caunt/Void/commit/55389106fcecc9e94693ad24da63929060b6b8f9))
* enhance moderation commands with suggestions ([4d25af9](https://github.com/caunt/Void/commit/4d25af940c3acf30556073f455c7cf565775479e))
* enhance PlatformService with command handling ([1044dab](https://github.com/caunt/Void/commit/1044dab69f5fab95b63ad3b284fec21eb659b347))
* enhance PlatformService with container commands ([24d7d8e](https://github.com/caunt/Void/commit/24d7d8ecbc3a90c5cc8a6e627087b1d5d744c25d))
* enhance plugin management with container support ([f84e44c](https://github.com/caunt/Void/commit/f84e44ca27bb275b0cb3e646ddb40005ecbce127))
* enhance RedirectionService with command handling ([36098cc](https://github.com/caunt/Void/commit/36098cc74374a27578e7c18ac92ca35ab48985b1))
* enhance service management and property handling ([0476ecb](https://github.com/caunt/Void/commit/0476ecbaa4af06fdcd7e582176c2ac34c04347d2))
* enhance service management in HostingExtensions ([5631775](https://github.com/caunt/Void/commit/56317750e3677576c00ea7d63608c0b29f2f3422))
* enhance service provider validation ([37d6481](https://github.com/caunt/Void/commit/37d6481c2fc8c4c4d16937db16ff6296e2dbc64f))
* enhance Uuid struct with comparison and equality ([c00b9e5](https://github.com/caunt/Void/commit/c00b9e5d9cae3ce83a5b1411110bd7fcc280efba))
* enhance WeakPluginContainer for better management ([ee796e3](https://github.com/caunt/Void/commit/ee796e361868d9bd7b81c4fef145473db5cfbd0f))
* **event:** add RegisterListener&lt;T&gt; method and duplicate check ([f6e7426](https://github.com/caunt/Void/commit/f6e74269d14d5a4aa2f266cc0ac7440992f17a77))
* **ExamplePlugin:** enhance service registration ([3257642](https://github.com/caunt/Void/commit/3257642f7f621ff5e9ab8dda9944ed2842a06002))
* **formatting:** set default formatting properties to false ([35ed244](https://github.com/caunt/Void/commit/35ed2443baeadb00859e8bd0eecd734a66881ebc))
* improve argument retrieval in CommandContext ([55d8d2f](https://github.com/caunt/Void/commit/55d8d2f9569b830513013a9825f1a325ec071ee8))
* improve segment handling in ComponentLegacySerializer ([42a680c](https://github.com/caunt/Void/commit/42a680cf3567231a9cc7dc12674a3ca4bc2f7e95))
* improve service instance creation and retrieval ([ce1295a](https://github.com/caunt/Void/commit/ce1295ae540abc157135a039d997570cff636274))
* instantiate example services from hosting abstractions, effectively using automatic registration of them as event listeners ([a6aa49e](https://github.com/caunt/Void/commit/a6aa49e5d8317eec81844d0d8290124e60dc2afc))
* integrate DryIoc for dependency injection ([4e7972d](https://github.com/caunt/Void/commit/4e7972dc8c9d808d93f76e04037e54b7a6375fd8))
* introduce IDependencyService for better dependency management ([a543fea](https://github.com/caunt/Void/commit/a543fea26ade2abb558c8e2484722d3f50876389))
* make IDependencyService inherit IServiceProvider ([72fcd2b](https://github.com/caunt/Void/commit/72fcd2b0dbe8d06d14a52f8306cb2a6e8bbd0bf3))
* move custom suggestions property to ArgumentCommandNode ([0d9aa60](https://github.com/caunt/Void/commit/0d9aa60e7bf91a657b00808d7711a1f876a607e6))
* move to dry ioc container ([62a1800](https://github.com/caunt/Void/commit/62a1800a10927672c935a9a3a8bc5c88ef3e976c))
* per-assembly containers ([856d2e0](https://github.com/caunt/Void/commit/856d2e0e8b4a895586289c62e2d5a4c2a568f90e))
* per-player scoped services ([543bece](https://github.com/caunt/Void/commit/543becea9423bfca126686b2beabd6b824535cc4))
* **plugin:** enhance dependency loading mechanism ([f7ef966](https://github.com/caunt/Void/commit/f7ef9666c09807252d3ba529190399d125872ab1))
* **plugins:** add PluginExtensions for improved plugin retrieval ([e5150a5](https://github.com/caunt/Void/commit/e5150a5b601aaed66850d15dcae8fb64f0cd0ae2))
* register listeners in service configuration ([270b31b](https://github.com/caunt/Void/commit/270b31be5784a30ed1503bbc5b6d8a12519a1370))
* register text component transformations 1.21.4 =&gt; 1.21.5 ([bca74f6](https://github.com/caunt/Void/commit/bca74f638bf29df7774045da616e5b4190c4a9d4))
* rename constructor parameter in PluginAssemblyLoadContext ([2ec4f39](https://github.com/caunt/Void/commit/2ec4f399ac999cf3eeab59542e18b61f005cd8ce))
* replace service provider factory with DryIoc container ([5bd218d](https://github.com/caunt/Void/commit/5bd218d67a65932562491637e4498b8ce8bd2bf3))
* simplify ConfigurationService constructor ([9b36f10](https://github.com/caunt/Void/commit/9b36f10dac40cdc31a476581e107c269b3674f1e))
* simplify IServiceProvider.Add method ([b7fc0f7](https://github.com/caunt/Void/commit/b7fc0f7e03a4539a63c5e0f1654f53a25f2fb50a))
* split loading/loaded and unloading/unloaded plugin events ([6748323](https://github.com/caunt/Void/commit/67483238c58b58fb8b76505b80656eb92017c555))
* streamline command registration for "kick" ([45b2536](https://github.com/caunt/Void/commit/45b253672ea798651760f51c3f5d555c02c2d7c0))
* synchronize plugin events ([cbae8a2](https://github.com/caunt/Void/commit/cbae8a2b3ba7eb049b2181f19567d995ef796a3f))
* text component transformations 1.21.4 =&gt; 1.21.5 ([35c478f](https://github.com/caunt/Void/commit/35c478f9aab5f3cd0efd6cc138340e6567a1bc03))
* **trace:** add TraceService for message logging ([0a4118a](https://github.com/caunt/Void/commit/0a4118a7bddab1d7d7f8e335a564ff48b1bef7ea))
* update command handling and logging in AbstractCommandService ([304c38f](https://github.com/caunt/Void/commit/304c38fc46aa8cf3219b11c6da37e945c530200c))
* update CommandService for chat message handling ([0f9b7d7](https://github.com/caunt/Void/commit/0f9b7d759aee514a3ac94a4d8d1ba7ccf8c8c57d))
* update configuration file to new configuration service ([6c51c0d](https://github.com/caunt/Void/commit/6c51c0d7f056229c058022ed8d81599b494295f3))
* update dependency injection and service management ([de3f18a](https://github.com/caunt/Void/commit/de3f18a5e521275823eefcd759469c328961084e))
* update ExamplePlugin constructor for better logging ([7079149](https://github.com/caunt/Void/commit/7079149a7caad88f0a78fdf5705f47d27d78cd02))
* update IClickEventAction records with parameters ([0cee75f](https://github.com/caunt/Void/commit/0cee75fe165ffb0c9ff164364e5e2dd9a7947124))
* update ModerationService and PlatformService ([f2f7d6d](https://github.com/caunt/Void/commit/f2f7d6d43f4a67d34d8128377659fc34c0440093))
* update player link retrieval methods ([9df2fae](https://github.com/caunt/Void/commit/9df2fae88843537a412ceaff76400baaff94816f))
* update Plugin class to use dependency injection ([8d87944](https://github.com/caunt/Void/commit/8d8794459fe393c5ebe1bcb77bd38be7e82c71d8))
* update Property record constructor and remove Deconstruct ([09c17fb](https://github.com/caunt/Void/commit/09c17fb619070c93624fc8bd321855ca9996a445))
* update service retrieval in DependencyService ([de1ae86](https://github.com/caunt/Void/commit/de1ae860a55a368efb149f9643de975d7519d9f1))
* update StringRange to use range operator ([ab90cd4](https://github.com/caunt/Void/commit/ab90cd40eac6bfc22512e1041448e7d5b0b7b83d))
* update text components to 1.21.5 ([37945d0](https://github.com/caunt/Void/commit/37945d04b2694d4619f6548ccf17b7a5197bea50))


### Bug Fixes

* activate only singletones ([54994ba](https://github.com/caunt/Void/commit/54994babbf7ed03b295a7bb8ee0cfe5a487feea8))
* add error handling in SimpleNetworkStream ([b1b17a1](https://github.com/caunt/Void/commit/b1b17a1a839a914dfe35c25074287c769a23a5b6))
* allow example plugin unloading ([b298690](https://github.com/caunt/Void/commit/b2986906a9a1aa8f5dbef143e53b41bfcfa9b6a1))
* clear event listeners registered via dependency service ([feba68e](https://github.com/caunt/Void/commit/feba68e44c4097545c84f0b3586719c69666d409))
* correct method name for encryption response packet ([061d4bc](https://github.com/caunt/Void/commit/061d4bc77ae99ea45cf3e6dcc1e6a88532d790c8))
* correct typo in README.md ([d5d39e9](https://github.com/caunt/Void/commit/d5d39e9efdf12aa680d311fffcd2cb584a17f841))
* do not forward commands if they are coming from player and are forwarded ([2871111](https://github.com/caunt/Void/commit/28711119eb73a6d289b3d9ce297de334c5624b53))
* ensure thread safety in ConfigurationService ([5020154](https://github.com/caunt/Void/commit/5020154c632e4e8e9ab0636e8059e7085278ebc0))
* filter only outgoing chat messages ([2103152](https://github.com/caunt/Void/commit/2103152588feed1fb05164842d16b19a9f7ac98f))
* handle empty tag names in NbtReader ([6eabfd1](https://github.com/caunt/Void/commit/6eabfd1416b89c9d0e99a823d1f83eab8a66c900))
* improve plugin file retrieval order ([9738607](https://github.com/caunt/Void/commit/9738607805bf531ab7d06a5ac48e6b2c078e01fc))
* improve plugin service management in DependencyService ([08220ca](https://github.com/caunt/Void/commit/08220ca8e10ebc6b370763c401e706194c374283))
* improve service instance creation in DependencyService ([94af648](https://github.com/caunt/Void/commit/94af648ef93c312faad299529e8a18ccb1b9947e))
* improve service management in DependencyService ([99eb539](https://github.com/caunt/Void/commit/99eb539a109962b5218e50ee811b2aadb8a5fccd))
* improve substring extraction and readability ([94edd62](https://github.com/caunt/Void/commit/94edd62d0363bfcac51ecb102b4a9c8fe62ff090))
* improve substring handling in CommandSyntaxException ([d635c21](https://github.com/caunt/Void/commit/d635c2185e9ef9e46bc20c1666c73be81962326f))
* pass serilog factory into a child containers ([7668170](https://github.com/caunt/Void/commit/7668170d9a51d598ee4c99c19021dd9e3f87dc95))
* remove deprecated service removal from players scope ([427414f](https://github.com/caunt/Void/commit/427414faf81dcec40e018bbee5b67b81480c84b8))
* remove intercepting service provider ([5380b2a](https://github.com/caunt/Void/commit/5380b2a53a8388130728f64fcd1cb850f1ffbbc3))
* reorder method calls for plugin file processing ([098ee8b](https://github.com/caunt/Void/commit/098ee8bb0334bb86048e252d4f53e994ecd25cda))
* simplify EventService constructor and cancellation handling ([6873af2](https://github.com/caunt/Void/commit/6873af2239d0a0dcb1be23710b6acefedf44e843))
* simplify OnPluginUnload method in DependencyService ([1b164a1](https://github.com/caunt/Void/commit/1b164a112a8232da1bca59928230dee9a4dbeae3))
* streamline instance creation in DependencyService ([5e630c4](https://github.com/caunt/Void/commit/5e630c4e3904fcdc14eee69c197f59861ee5939a))
* uncomment previuos unloading problem ([3a11604](https://github.com/caunt/Void/commit/3a11604fcf2dce09bcefb3984b9dd80dcbc04e37))
* update exception messages for registry management ([580e2bd](https://github.com/caunt/Void/commit/580e2bddd83d24d0f10090949a3e303ce6bfeb4f))
* update instance creation logic in DependencyService ([e20e38c](https://github.com/caunt/Void/commit/e20e38c94a6351085582d033c7222b215cd0e159))
* update mapping version comparison logic ([62ee5bb](https://github.com/caunt/Void/commit/62ee5bb04c79051bca49b9d4684715352a5910e3))
* update service access in DependencyService ([39f8c2c](https://github.com/caunt/Void/commit/39f8c2c54c0b45a4a28f311d37070cda8cd609ee))

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
