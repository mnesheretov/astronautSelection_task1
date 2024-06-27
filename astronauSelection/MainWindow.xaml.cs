using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace astronauSelection
{
  /// <summary>
  /// Логика взаимодействия для MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private List<Test> _tests;

    public MainWindow()
    {
      InitializeComponent();
      InitializeTests();
    }

    private void InitializeTests()
    {
      _tests = new List<Test>
      {
                new Test { Name = "Вес", Evaluation = candidate => EvaluateWeight(candidate.Weight) },
                new Test { Name = "Рост", Evaluation = candidate => EvaluateHeight(candidate.Height) },
                new Test { Name = "Возраст", Evaluation = candidate => EvaluateAge(candidate.Age) },
                new Test { Name = "Зрение", Evaluation = candidate => EvaluateVision(candidate.Vision) },
                new Test { Name = "Курение", Evaluation = candidate => EvaluateSmoking(candidate.HabitsAndDiseases) },
                new Test { Name = "Терапевт", Evaluation = candidate => EvaluateTherapist(candidate.HabitsAndDiseases) },
                new Test { Name = "Психолог", Evaluation = candidate => EvaluatePsychologist(candidate.HabitsAndDiseases) },
                new Test { Name = "Странный", Evaluation = candidate => EvaluateStrange(candidate.Age, candidate.Name) },
                new Test { Name = "Математический", Evaluation = candidate => EvaluateMath(candidate.HabitsAndDiseases, candidate.Height) }
            };
    }

    private void OnCheckCandidateClick(object sender, RoutedEventArgs e)
    {
      string name = NameTextBox.Text;
      if (string.IsNullOrWhiteSpace(name)) 
      {
        MessageBox.Show("Имя не должно быть пустым.");
        return;
      }

      if (!int.TryParse(WeightTextBox.Text, out int weight) || weight <= 0)
      {
        MessageBox.Show("Некорректный вес.");
        return;
      }

      if (!int.TryParse(HeightTextBox.Text, out int height) || height <= 0)
      {
        MessageBox.Show("Некорректный рост.");
        return;
      }

      if (!int.TryParse(AgeTextBox.Text, out int age) || age <= 0)
      {
        MessageBox.Show("Некорректный возраст.");
        return;
      }

      if (!double.TryParse(VisionTextBox.Text, out double vision) || vision < 0 || vision > 1)
      {
        MessageBox.Show("Некорректное зрение.");
        return;
      }

      string[] habitsAndDiseases = HabitsAndDiseasesTextBox.Text.Split(' ');

      Candidate candidate = new Candidate
      {
        Name = name,
        Weight = weight,
        Height = height,
        Age = age,
        Vision = vision,
        HabitsAndDiseases = new HashSet<string>(habitsAndDiseases)
      };

       List<string> unsatisfactoryResults = new List<string>();
       int satisfactoryCount = 0;

       foreach (var test in _tests)
       {
           string result = test.Evaluation(candidate);

           if (result.Contains("(неудовлетворительно)"))
           {
               unsatisfactoryResults.Add(result);
           }
           else if (result.Contains("(удовлетворительно)"))
           {
               satisfactoryCount++;
           }
       }

       if (unsatisfactoryResults.Count > 0 || satisfactoryCount >= 3)
       {
           ResultTextBlock.Text = $"Кандидат {candidate.Name} не прошел тестирование. Проблемы:\n" + string.Join("\n", unsatisfactoryResults.Select(p => $"* {p}"));
       }
       else
       {
           ResultTextBlock.Text = $"Кандидат {candidate.Name} подходит";
       }
    }

    private void OnNextCandidateClick(object sender, RoutedEventArgs e)
    {
      NameTextBox.Clear();
      WeightTextBox.Clear();
      HeightTextBox.Clear();
      AgeTextBox.Clear();
      VisionTextBox.Clear();
      HabitsAndDiseasesTextBox.Clear();
      ResultTextBlock.Text = string.Empty;
    }


    private string EvaluateWeight(int weight)
    {
      if (weight < 70 || weight > 100)
      {
        return $"Вес кандидата {weight} кг (неудовлетворительно)";
      }
      if ((weight >= 70 && weight < 75) || (weight > 90 && weight <= 100))
      {
        return $"Вес кандидата {weight} кг (удовлетворительно)";
      }
      return string.Empty; 
    }

    private string EvaluateHeight(int height)
    {
      if (height < 160 || height > 190)
      {
        return $"Рост кандидата {height} см (неудовлетворительно)";
      }
      if ((height >= 160 && height < 170) || (height > 185 && height <= 190))
      {
        return $"Рост кандидата {height} см (удовлетворительно)";
      }
      return string.Empty; 
    }

    private string EvaluateAge(int age)
    {
      if (age < 23 || age > 37)
      {
        return $"Возраст кандидата {age} лет (неудовлетворительно)";
      }
      if ((age >= 23 && age < 25) || (age > 35 && age <= 37))
      {
        return $"Возраст кандидата {age} лет (удовлетворительно)";
      }
      return string.Empty; 
    }

    private string EvaluateVision(double vision)
    {
      if (vision < 1)
      {
        return $"Зрение кандидата {vision} (неудовлетворительно)";
      }
      return string.Empty; 
    }

    private string EvaluateSmoking(HashSet<string> habitsAndDiseases)
    {
      if (habitsAndDiseases.Contains("курение"))
      {
        return "Кандидат курит (неудовлетворительно)";
      }
      return string.Empty; 
    }

    private string EvaluateTherapist(HashSet<string> habitsAndDiseases)
    {
      int diseaseCount = 0;
      string[] therapistDiseases = { "насморк", "бронхит", "вирус", "аллергия", "ангина", "бессонница" };
      foreach (var disease in therapistDiseases)
      {
        if (habitsAndDiseases.Contains(disease))
        {
          diseaseCount++;
        }
      }

      if (diseaseCount > 3)
      {
        return $"Терапевт выявил {diseaseCount} болезни (неудовлетворительно)";
      }
      if (diseaseCount == 3)
      {
        return $"Терапевт выявил {diseaseCount} болезни (удовлетворительно)";
      }
      return string.Empty; 
    }

    private string EvaluatePsychologist(HashSet<string> habitsAndDiseases)
    {
      int diseaseCount = 0;
      string[] psychologistDiseases = { "алкоголизм", "бессонница", "наркомания", "травмы" };
      foreach (var disease in psychologistDiseases)
      {
        if (habitsAndDiseases.Contains(disease))
        {
          diseaseCount++;
        }
      }

      if (diseaseCount > 1)
      {
        return $"Психолог выявил {diseaseCount} болезни (неудовлетворительно)";
      }
      if (diseaseCount == 1)
      {
        return $"Психолог выявил {diseaseCount} болезнь (удовлетворительно)";
      }
      return string.Empty; 
    }

    private string EvaluateStrange(int age, string name)
    {
      if (name[0] == 'П')
            {
                return string.Empty;
            }
      else if (age > 68)
            {
                return $"Имя не начинается на 'П' и возраст более 60 (удоволетворительно)";
            }
      else
            {
                return $"Кандидат не прошёл тест 'Странный' (неудовлетворительно)";
            }
    }


    private string EvaluateMath(HashSet<string> habitsAndDiseases, int height)
    {
        if (height % 3 == 0 && (habitsAndDiseases.Contains("насморк") || habitsAndDiseases.Contains("Насморк")))
            {
                return $"Кандидат не прошёл тест 'Математический' (неудовлетворительно)";
            }
        else if (height % 2 == 0)
            {
                return string.Empty;
            }
        else
            {
                return $"Кандидат прошёл тест 'Математический' (удовлетворительно)";
            }
    }
    }

  public class Candidate
  {
    public string Name { get; set; }
    public int Weight { get; set; }
    public int Height { get; set; }
    public int Age { get; set; }
    public double Vision { get; set; }
    public HashSet<string> HabitsAndDiseases { get; set; }
  }

  public class Test
  {
    public string Name { get; set; }
    public Func<Candidate, string> Evaluation { get; set; }
  }
}

