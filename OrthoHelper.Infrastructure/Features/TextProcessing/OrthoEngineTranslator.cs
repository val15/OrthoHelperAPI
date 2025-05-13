using Microsoft.Extensions.Logging;
using OrthoHelper.Domain.Features.Common.Ports;
using OrthoHelper.Domain.Features.TextCorrection.Ports.Repositories;

namespace OrthoHelper.Infrastructure.Features.TextProcessing
{

    public class OrthoEngineTranslator : OrthoEngine
    {
        public override string InitText => "Veuillez traduire le texte suivant en français en respectant impérativement les règles suivantes :\r\n\r\nTraduction avec préservation : Traduisez le contenu général du texte en français.\r\nNe pas traduire les termes spécifiés : Les mots-clés, les noms techniques (par exemple, noms de logiciels, composants matériels, termes de programmation, jargon scientifique ou technique spécifique, noms de marques, etc.) doivent impérativement être conservés en anglais tels quels dans le texte français final. Ne les traduisez sous aucun prétexte.\r\nFallback à l'original si intraduisible ou ambigu : Si un mot ou une expression (qui n'est PAS un mot-clé ou un nom technique à conserver) s'avère particulièrement difficile à traduire, si sa traduction est ambiguë, ou si vous estimez que la traduction pourrait dénaturer un concept important dans le respect des règles ci-dessus, alors conservez ce mot ou cette expression spécifique en anglais dans la phrase française.\r\nCohérence du résultat : Vous devez systématiquement retourner un texte complet. Il n'y aura pas de message d'erreur pour un mot non traduit ; le mot original sera utilisé à la place comme indiqué ci-dessus.\r\nQualité du français pour le reste : Pour toutes les parties du texte qui sont effectivement traduites en français, assurez-vous que le français est clair, grammaticalement correct et sonne naturel, tout en essayant de refléter l'intention de l'auteur pour ces segments.\r\nL'objectif est d'obtenir un texte français fonctionnel et compréhensible pour un public qui reconnaîtrait et s'attendrait à voir la terminologie technique et les mots-clés critiques en anglais, afin d'éviter toute confusion ou perte de précision.";


        //TODO IN ENV OU PARAM
        public override string BottomOfThequestion => "Veuillez traduire le texte anglais suivant en français. Je souhaite une traduction qui ne soit pas strictement littérale mot à mot. L'objectif principal est de transmettre fidèlement le sens, l'intention, les nuances et le ton de l'auteur original.\r\n\r\nAssurez-vous que le français produit soit naturel, idiomatique et grammaticalement impeccable. Il est crucial de prendre en compte l'intégralité du contexte fourni avec ce texte (par exemple, le type de document, le public cible, l'objectif général, etc.).\r\n\r\nSi certaines parties du texte sont ambiguës, particulièrement idiomatiques à l'anglais, ou contiennent des références culturelles spécifiques qui rendent une traduction directe difficile tout en conservant le sens, veuillez utiliser votre meilleur jugement pour les adapter de manière appropriée au public francophone. Si un point est particulièrement critique ou complexe à transposer, n'hésitez pas à le signaler brièvement.";

        public OrthoEngineTranslator(HttpClient httpClient, ISessionRepository repository, ICurrentUserService currentUserService, ILogger<OrthoEngineTranslator> logger)
            : base(httpClient, repository, currentUserService, logger)
        {

        }
    }
}
