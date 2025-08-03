using System.IO.Ports;

using Application.Interfaces;
using Application.Models;

namespace Infrastructure.Hardware;

public class Rs485SensorClient : ISensorClient
{
    public bool IsConnected { get; private set; }
    
    private const string SendCommand = "SEND\r\n";
    
    private readonly SerialPort _serialPort;
    private readonly HashSet<int> _validBaudRates =  [ 75, 110, 134, 300, 600, 1200, 1800, 2400, 4800, 7200, 9600, 14400, 19200, 38400, 57600, 115200, 128000 ];
    
    public Rs485SensorClient(string portName, int baudRate, string parity, int dataBits, string stopBits, int readTimeout = 5000, int writeTimeout = 5000)
    {
        if (string.IsNullOrWhiteSpace(portName))
        {
            throw new ArgumentException("Port name cannot be null or empty.", nameof(portName));
        }
        
        if (!_validBaudRates.Contains(baudRate))
        {
            throw new ArgumentOutOfRangeException(nameof(baudRate), $"Invalid baud rate. Supported rates: {string.Join(", ", _validBaudRates)}");
        }
        
        if (dataBits is < 4 or > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(dataBits), "Data bits must be between 4 and 8.");
        }
        
        if (!Enum.TryParse<Parity>(parity, true, out var parsedParity))
        {
            throw new ArgumentException($"Invalid parity value: {parity}. Supported values: None, Odd, Even, Mark, Space.", nameof(parity));
        }
        
        if (!Enum.TryParse<StopBits>(stopBits, true, out var parsedStopBits))
        {
            throw new ArgumentException($"Invalid stop bits value: {stopBits}. Supported values: None, One, OnePointFive, Two.", nameof(stopBits));
        }
        
        _serialPort = new SerialPort
        {
            PortName = portName,
            BaudRate = baudRate,
            Parity = parsedParity,
            DataBits = dataBits,
            StopBits = parsedStopBits,
            WriteTimeout = writeTimeout,
            ReadTimeout = readTimeout
        };
    }
    public Task<bool> ConnectAsync(CancellationToken cancellationToken)
    {
        try
        {
            _serialPort.Open();
            IsConnected = true;
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            IsConnected = false;
            throw new InvalidOperationException("Failed to connect to RS485 sensor.", ex);
        }
    }

    public Task DisconnectAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }
            IsConnected = false;
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to disconnect from RS485 sensor.", ex);
        }
    }

    public Task<RawSensorReading> GetReadingAsync(CancellationToken cancellationToken)
    {
        if (!IsConnected)
        {
            throw new InvalidOperationException("Sensor is not connected.");
        }

        try
        {
           _serialPort.Write(SendCommand);
           var response = _serialPort.ReadLine();
           if (string.IsNullOrWhiteSpace(response))
           {
               throw new InvalidOperationException("No response received from RS485 sensor.");
           }
           
           
            var reading = CleanReading(response);
            return Task.FromResult(reading);
           
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to read from RS485 sensor.", ex);
        }
    }

    private static RawSensorReading CleanReading(string reading)
    {
        var parts = reading.Split('\t')
            .ToList()
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => double.TryParse(x, out var value) ? value : throw new FormatException($"Invalid number format: {x}"))
            .ToList();
        
        if (parts.Count != 3)
        {
            throw new InvalidOperationException("Invalid reading format.");
        }
        
        return new RawSensorReading
        {
            Temperature = parts[0],
            DewPoint = parts[1],
            Humidity = parts[2],
        };
    }
}
