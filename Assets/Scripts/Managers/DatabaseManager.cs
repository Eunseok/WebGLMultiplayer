using System;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static DatabaseManager Instance { get; private set; }

    FirebaseService _firebaseService;
    UserService _userService;
    MessageService _messageService;

    [SerializeField] string databaseUrl = "https://multiplayergame-eec4e-default-rtdb.firebaseio.com";

    void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 이미 인스턴스가 있으면 파괴
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 다른 씬에서도 유지

        InitializeServices();
    }

    void InitializeServices()
    {
        // FirebaseService 초기화
        _firebaseService = new FirebaseService(databaseUrl);

        // 각 서비스 초기화
        _userService = new UserService(_firebaseService);
        _messageService = new MessageService(_firebaseService);
    }

    // 전역 서비스 접근자
    public UserService UserService => _userService;
    public MessageService MessageService => _messageService;
}